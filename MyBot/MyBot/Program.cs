using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

using ApiAiSDK;
using ApiAiSDK.Model;

namespace MyBot
{
    class Program
    {
        static TelegramBotClient Bot;
        static ApiAi apiAi;

        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("1054346504:AAHJl26gxCdL-UrPirXfuqKwLkL9Xt4ZOyI");
            AIConfiguration config = new AIConfiguration("d2ff912985d04e01ac1dfab22e080762", SupportedLanguage.Russian);
            apiAi = new ApiAi(config);

            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} нажал на кнопку {buttonText}");

            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопку {buttonText}");
            
        }

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message.Type != MessageType.Text || message == null)
                return;
            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{name} отправил сообщение6: '{message.Text}'");
            switch (message.Text )
            {
                case "/help":
                    string text =
@"Список команд:
/inline - вывод меня
/keyboard - вывод клавиатуры";
                    await Bot.SendTextMessageAsync(message.From.Id, text);
                break;

                case "/inline":
                    var inlineKeyboerd = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Мемасный телега :)", "https://t.me/darckness_reveur"),
                            InlineKeyboardButton.WithUrl("Админ телега:)", "https://t.me/darckness_reveur")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Мемасный вк", "https://vk.com/epritula99"),
                            InlineKeyboardButton.WithUrl("Админ вк", "https://vk.com/darkness_reveur")
                        }
                    }) ;
                    await Bot.SendTextMessageAsync(message.From.Id, "Выберите пункт меню",
                        replyMarkup: inlineKeyboerd);
                    break;

                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("Геолакация") { RequestLocation = true},
                        new KeyboardButton("Контакты") { RequestContact = true}

                    });

                    await Bot.SendTextMessageAsync(message.Chat.Id, "Если что-то не напишу, фиганёт ошибку (", replyMarkup: replyKeyboard);
                    break;

                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == null || answer == "")
                        answer = "Прости, я тебя не понимаю";
                    await Bot.SendTextMessageAsync(message.From.Id, answer);
                    break;
            }
        }
    }
}
