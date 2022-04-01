using Newtonsoft.Json;

namespace PxStat.Subscription
{
    internal class Query_DTO_Create
    {
        public string Uid { get; set; }
        public string AccessToken { get; set; }
        public string TagName { get; set; }
        public string Matrix { get; set; }
        public string SubscriberUserId { get; set; }
        public Snippet Snippet { get; set; }

        public Query_DTO_Create(dynamic parameters)
        {
            if (parameters.Uid != null)
            {
                Uid = parameters.Uid;
            }
            if (parameters.AccessToken != null)
            {
                AccessToken = parameters.AccessToken;
            }
            if (parameters.TagName != null)
            {
                TagName = parameters.TagName;
            }
            if (parameters.Matrix != null)
            {
                Matrix = parameters.Matrix;
            }
            if (parameters.SubscriberUserId != null)
            {
                SubscriberUserId = parameters.SubscriberUserId;
            }
            if (parameters.Snippet != null)
            {
                Snippet = JsonConvert.DeserializeObject<Snippet>(parameters.Snippet.ToString());
            }
        }

        public Query_DTO_Create()
        {
        }
    }

    internal class Snippet
    {
        public string Type { get; set; }
        public string Query { get; set; }
        public string Isogram { get; set; }
        public bool FluidTime { get; set; }
    }

    internal class Query_DTO_Delete
    {
        public string Uid { get; set; }
        public string AccessToken { get; set; }

        public string UserQueryId { get; set; }
        public Query_DTO_Delete(dynamic parameters)
        {
            if (parameters.Uid != null)
            {
                Uid = parameters.Uid;
            }
            if (parameters.AccessToken != null)
            {
                AccessToken = parameters.AccessToken;
            }
            if (parameters.UserQueryId != null)
            {
                UserQueryId = parameters.UserQueryId;
            }
        }
    }

    internal class Query_DTO_Read
    {
        public string Uid { get; set; }
        public string AccessToken { get; set; }
        public string UserQueryId { get; set; }
        public string SubscriberUserId { get; set; }
        public Query_DTO_Read(dynamic parameters)
        {
            if (parameters.Uid != null)
            {
                Uid = parameters.Uid;
            }
            if (parameters.AccessToken != null)
            {
                AccessToken = parameters.AccessToken;
            }
            if (parameters.UserQueryId != null)
            {
                UserQueryId = parameters.UserQueryId;
            }
            if (parameters.SubscriberUserId != null)
            {
                SubscriberUserId = parameters.SubscriberUserId;
            }
        }
    }
    internal class Query_DTO_ReadAll
    {
        public string Uid { get; set; }
        public string AccessToken { get; set; }
        public string SubscriberUserId { get; set; }
        public Query_DTO_ReadAll(dynamic parameters)
        {
            if (parameters.Uid != null)
            {
                Uid = parameters.Uid;
            }
            if (parameters.AccessToken != null)
            {
                AccessToken = parameters.AccessToken;
            }
            if (parameters.SubscriberUserId != null)
            {
                SubscriberUserId = parameters.SubscriberUserId;
            }
        }
    }
}
