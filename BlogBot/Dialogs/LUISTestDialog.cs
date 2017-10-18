﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogBot.Dialogs
{
    using System.Threading.Tasks;

    using BlogBot.Models;

    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;

    [Serializable]
    [LuisModel("7be6cff2-4439-4a81-83b6-5c60147fafdc", "ffa1ff3be1d4490b9df4c62299a81715")]
    public class LUISTestDialog : LuisDialog<BlogComment>
    {
        [LuisIntent("")]
        public async Task NoIntentFound(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("LUIS could not find a matching intent.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Hello Intent")]
        public async Task HelloIntent(IDialogContext context, LuisResult result)
        {
            context.Call(new HelloDialog(), Callback);
            async Task Callback(IDialogContext dialogContext, IAwaitable<object> dialogResult)
            {
                dialogContext.Wait(this.MessageReceived);
            }
        }

        public async Task BlogAspects(IDialogContext context, LuisResult result)
        {
            var newForm = new FormDialog<BlogComment>(new BlogComment(), this.AcceptComment, FormOptions.PromptInStart);
            context.Call(newForm, Continue);
            async Task Continue(IDialogContext dialogContext, IAwaitable<object> dialogResult)
            {
                dialogContext.Wait(this.MessageReceived);
            }
        }

        private IForm<BlogComment> AcceptComment()
        {
            throw new NotImplementedException();
        }

        [LuisIntent("Blog Aspects")]
        public async Task CanCommentOn(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(e => e.Type == "Blog Aspect Entity"))
            {
                var name = entity.Entity.ToLowerInvariant();
                if (name == "blog" || name == "profile")
                {
                    await context.PostAsync("Yes you can comment on name");
                    context.Wait(this.MessageReceived);
                    return;
                }
            }

            await context.PostAsync("Not an available option");
            context.Wait(this.MessageReceived);
        }
    }
}