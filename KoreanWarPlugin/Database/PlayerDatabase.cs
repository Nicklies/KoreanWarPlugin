using KoreanWarPlugin.Data;
using KoreanWarPlugin.KWSystem;
using KoreanWarPlugin.Storage;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Database
{
    public class PlayerDatabase
    {
        private DataStorage<List<PlayerData>> DataStorage_PlayerData { get; set; }
        public List<PlayerData> Data_Player;
        public PlayerDatabase()
        {
            DataStorage_PlayerData = new DataStorage<List<PlayerData>>(PluginManager.instance.Directory, "PlayerData.json");
        }

        public void Reload()
        {
            Data_Player = DataStorage_PlayerData.Read();
            if (Data_Player == null)
            {
                Data_Player = new List<PlayerData>();
                DataStorage_PlayerData.Save(Data_Player);
            }
        }
        public void AddData(PlayerData _data)
        {
            Data_Player.Add(_data);
        }
        public void RemoveData(PlayerData _data)
        {
            Data_Player.Remove(_data);
        }
        public void UpdateData()
        {
            DataStorage_PlayerData.Save(Data_Player);
        }
        public PlayerData FindData(CSteamID _cSteamID)
        {
            PlayerData playerData = Data_Player.FirstOrDefault(x => x.cSteamID == _cSteamID);
            if (playerData != default) return playerData;
            else
            {
                playerData = new PlayerData(_cSteamID);
                PluginManager.playerDatabase.AddData(playerData);
                return playerData;
            }
        }
        public void GiveCredit(UnturnedPlayer _uPlayer,uint _amount) // 라운드가 끝날 시 실행되는 함수
        {
            if (_amount == 0) return;
            PlayerData pData = PluginManager.playerDatabase.FindData(_uPlayer.CSteamID);
            if (pData == null) return;
            pData.credit += _amount;
            PluginManager.playerDatabase.UpdateData();
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            if(pc.localUIState == EnumTable.EPlayerUIState.Loadout)
            {
                ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
                VehicleGroupSystem.RefreshVehicleTypeStateAll(tc, pc.team, _uPlayer); // 차량 조건문 갱신
                LoadoutSystem.RefreshUICredit(tc, pData);
            }
        }
    }
}
