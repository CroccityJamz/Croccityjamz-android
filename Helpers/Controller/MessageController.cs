namespace DeepSound.Helpers.Controller
{
    public class MessageController
    {
        //############# DON'T MODIFY HERE #############
        //========================= Functions =========================

        //public static async Task SendMessageTask(int userId, string text, string stickerId, string path, string hashId, UserInfoObject userData)
        //{
        //    try
        //    {
        //        var (apiStatus, respond) = await RequestsAsync.Chat.SendMessageAsync(userId.ToString(), text, stickerId, path, hashId);
        //        if (apiStatus == 200)
        //        {
        //            if (respond is SendMessageObject result)
        //            {
        //                if (result.data != null)
        //                {
        //                    UpdateLastIdMessage(result.data, userData);
        //                }
        //            }
        //        }
        //        else if (apiStatus == 400)
        //        {
        //            if (respond is ErrorObject error)
        //            {
        //                var errorText = error.ErrorData.ErrorText;
        //                Toast.MakeText(Application.Context, errorText, ToastLength.Short);
        //            }
        //        }
        //        else if (apiStatus == 404)
        //        {
        //            var error = respond.ToString();
        //            Toast.MakeText(Application.Context, error, ToastLength.Short);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        //public static void UpdateLastIdMessage(SendMessageObject.Data messages, UserInfoObject userData)
        //{
        //    try
        //    {
        //        var checker = MessagesBoxActivity.MAdapter.MessageList.FirstOrDefault(a => a.Id == Convert.ToInt32(messages.HashId));
        //        if (checker != null)
        //        {
        //            checker.Id = messages.Id;
        //            checker.FromName = UserDetails.FullName;
        //            checker.FromAvater = UserDetails.Avatar;
        //            checker.ToName = userData?.Fullname ?? "";
        //            checker.ToAvater = userData?.Avater ?? "";
        //            checker.From = messages.From;
        //            checker.To = messages.To;
        //            checker.Text = messages.Text;
        //            checker.Media = messages.Media;
        //            checker.FromDelete = messages.FromDelete;
        //            checker.ToDelete = messages.ToDelete;
        //            checker.Sticker = messages.Sticker;
        //            checker.CreatedAt = messages.CreatedAt;
        //            checker.Seen = messages.Seen;
        //            checker.Type = "Sent";
        //            checker.MessageType = messages.MessageType;

        //            string text = messages.Text;

        //            switch (checker.MessageType)
        //            {
        //                case "text":
        //                {
        //                    text = string.IsNullOrEmpty(text) ? Application.Context.GetText(Resource.String.Lbl_SendMessage) : messages.Text;
        //                    break;
        //                }
        //                case "media":
        //                {
        //                    text = Application.Context.GetText(Resource.String.Lbl_SendImageFile);
        //                    break;
        //                }
        //                case "sticker" when checker.Sticker.Contains(".gif"):
        //                {
        //                    text = Application.Context.GetText(Resource.String.Lbl_SendGifFile);
        //                    break;
        //                }
        //                case "sticker":
        //                {
        //                    text = Application.Context.GetText(Resource.String.Lbl_SendStickerFile);
        //                    break;
        //                }
        //            }

        //            var dataUser = LastChatActivity.MAdapter.UserList?.FirstOrDefault(a => a.User.Id == messages.To);
        //            if (dataUser != null)
        //            { 
        //                var index = LastChatActivity.MAdapter.UserList?.IndexOf(LastChatActivity.MAdapter.UserList?.FirstOrDefault(x => x.User.Id == messages.To));
        //                if (index > -1)
        //                { 
        //                    dataUser.Text = text;

        //                    LastChatActivity.MAdapter.Move(dataUser);
        //                    LastChatActivity.MAdapter.Update(dataUser);
        //                }
        //            }
        //            else
        //            {
        //                if (userData != null)
        //                { 
        //                    LastChatActivity.MAdapter.Insert(new GetConversationListObject.Data()
        //                    {
        //                        Id = userData.Id,
        //                        Owner = 0,
        //                        User = userData,
        //                        Seen = 1,
        //                        Text = text,
        //                        Media = messages.Media,
        //                        Sticker = messages.Sticker,
        //                        Time = messages.CreatedAt,
        //                        CreatedAt = userData.CreatedAt,
        //                        NewMessages = 0
        //                    });
        //                }
        //            }

        //            SqLiteDatabase dbDatabase = new SqLiteDatabase();
        //            GetChatConversationsObject.Messages message = new GetChatConversationsObject.Messages
        //            {
        //                Id = messages.Id,
        //                FromName = UserDetails.FullName,
        //                FromAvater = UserDetails.Avatar,
        //                ToName = userData?.Fullname ?? "",
        //                ToAvater = userData?.Avater ?? "",
        //                From = messages.From,
        //                To = messages.To,
        //                Text = messages.Text,
        //                Media = messages.Media,
        //                FromDelete = messages.FromDelete,
        //                ToDelete = messages.ToDelete,
        //                Sticker = messages.Sticker,
        //                CreatedAt = messages.CreatedAt,
        //                Seen = messages.Seen,
        //                Type = messages.Type,
        //                MessageType = messages.MessageType, 
        //            };
        //            //Update All data users to database
        //            dbDatabase.InsertOrUpdateToOneMessages(message);
        //            dbDatabase.Dispose();

        //            MessagesBoxActivity.UpdateOneMessage(message);

        //            if (AppSettings.RunSoundControl)
        //                IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

    }
}