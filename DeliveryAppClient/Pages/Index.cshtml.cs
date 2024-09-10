using DeliveryApp.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace DeliveryAppClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ServiceBusQueue _serviceBusQueue;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            _serviceBusQueue = new ServiceBusQueue("");
            Answers = new List<string>();
        }

        public async Task OnGet()
        {
            this.Answers = new List<string>(await _serviceBusQueue.ReceiveMessagesAsync());
            /*await _serviceBusQueue.SetupProcessor(
                args => {
                    RedirectToPage("Error");
                    return Task.CompletedTask;
                },
                msg => {
                    //RedirectToPage("Index");
                    return msg.CompleteMessageAsync(msg.Message);
                });*/
        }

        [BindProperty]
        public string Message { get; set; }

        public List<string> Answers { get; set; }

        public async Task OnPostAsync()
        {
            await _serviceBusQueue.SendMessageAsync(Message);
        }
    }
}
