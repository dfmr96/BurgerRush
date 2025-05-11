using System.Threading.Tasks;
using Save;

namespace Services.Cloud
{
    public static class CloudNicknameHandler
    {
        private const string CloudKey = "PlayerNickname";

        public static async Task SaveNickname(string nickname)
        {
            var data = new PlayerNameData { nickname = nickname };
            await CloudSaveEntity<PlayerNameData>.Save(CloudKey, data);
        }

        public static async Task<string> LoadNickname()
        {
            var data = await CloudSaveEntity<PlayerNameData>.Load(CloudKey);
            return data.nickname;
        }

        public static async Task DeleteNickname()
        {
            await CloudSaveEntity<PlayerNameData>.Delete(CloudKey);
        }
    }
}