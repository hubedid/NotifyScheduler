using System.Net;
using System.Text.Json;
using RestSharp;

namespace Notify.Repository.Email;

public class EmailSenderRepository : IEmailSenderRepository
{
    private readonly IConfiguration _configuration;

    public EmailSenderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendEmail(string email, string body)
    {

        var data = new Dictionary<string, object>
        {
            { "from",
                new {
                    email = "reminder.notify@bantuin.me",
                    name = "Notify-Me"
                }
            },
            { "to",
                new [] {
                    new {
                        email,
                    },
                }
            },
            { "template_uuid", "47950901-7715-4569-8813-9f1eb671fdbd" },
            { "template_variables",
                new {
                    message = body,
                }
            },
        };

        var client = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri("https://send.api.mailtrap.io/api/send"),
        });


        var request = new RestRequest();
        request.AddHeader("Authorization", "Bearer " + _configuration["MailTrap:Key"]);
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", JsonSerializer.Serialize(data), ParameterType.RequestBody);
        request.Method = Method.Post;

        RestResponse response = await client.ExecuteAsync(request);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        return true;

    }
}
