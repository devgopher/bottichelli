using Botticelli.Framework.Controls.BasicControls;
using Botticelli.Framework.Controls.Exceptions;
using Botticelli.Framework.Controls.Extensions;
using Botticelli.Framework.Controls.Layouts;
using Botticelli.Framework.Vk.Messages.API.Markups;
using Botticelli.Shared.Utils;
using Action = Botticelli.Framework.Vk.Messages.API.Markups.Action;

namespace Botticelli.Framework.Vk.Messages.Layout;

public class VkLayoutSupplier : IVkLayoutSupplier
{
    public VkKeyboardMarkup GetMarkup(ILayout layout)
    {
        if (layout == default)
            throw new LayoutException("Layout = null!");

        layout.Rows.NotNull();
        
        var buttons = new List<List<VkItem>>(10);

        foreach (var layoutRow in layout.Rows.EmptyIfNull())
        {
            var keyboardElement = new List<VkItem>();

            keyboardElement.AddRange(layoutRow.Items.Where(i => i.Control != default)
                .Select(item =>
            {
                item.Control.NotNull();
                item.Control.Content.NotNull();
                item.Control.MessengerSpecificParams.NotNull();
                
                var controlParams = item.Control.MessengerSpecificParams.ContainsKey("VK") ? item.Control?.MessengerSpecificParams["VK"] 
                        : new Dictionary<string, object>();
                
                controlParams.NotNull();
                
                var action = new Action
                {
                    Type = item.Control is TextButton ? "text" : "button",
                    Payload = $"{{\"button\": \"{layout.Rows.IndexOf(layoutRow)}\"}}",
                    Label = item.Control!.Content,
                    AppId = controlParams.ReturnValueOrDefault<int>("AppId"),
                    OwnerId = controlParams.ReturnValueOrDefault<int>("OwnerId"),
                    Hash = controlParams.ReturnValueOrDefault<string>("Hash")
                };

                return new VkItem
                {
                    Action = action,
                    Color = controlParams.ReturnValueOrDefault<string>("Color")
                };
            }));
            
            buttons.Add(keyboardElement);
        }

        var markup = new VkKeyboardMarkup()
        {
            OneTime = true,
            Buttons = buttons
        };

        return markup;
    }
}