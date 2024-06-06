using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.KWSystem;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Info
{
    public class VehicleGroupInfo // 유저가 차량 배치 후 생기는 차량 그룹 정보
    {
        public ushort instanceID;
        public CSteamID leaderID; // 그룹 리더의 아이디
        public VehicleTypePresetTable vehicleTypePreset; // 차종정보
        public byte vTypeIndex; // 해당 차량 그룹의 차량 종류 인덱스
        public SeatInfo[] seats;
        public byte crewCount; // 현재 차량 인원 수
        public byte maxSeats; // 최대 입장 가능한 유저 수
        public bool isLocked;
        public string password;
        // 차량 로드아웃 정보
        public ushort vPresetIndex { get; set; } // 선택된 차량 인덱스
        public ushort vAmmoIndex_0 { get; set; }
        public ushort vAmmoIndex_1 { get; set; }

        public VehicleGroupInfo(VehicleTypePresetTable _vTypePreset, byte _vIndex, byte _vTypeIndex, CSteamID _Leader)
        {
            //123
            instanceID = 0;
            vehicleTypePreset = _vTypePreset;
            vTypeIndex = _vTypeIndex;
            vPresetIndex = _vTypePreset.vehicleList[0];
            VehiclePresetTable vehiclePreset = PluginManager.instance.Configuration.Instance.vehiclePresets[_vTypePreset.vehicleList[_vIndex]];
            seats = new SeatInfo[vehiclePreset.seats.Length];
            for (int i = 0; i < seats.Length; i++) { seats[i] = new SeatInfo(); }
            maxSeats = (byte)seats.Length;
            seats[0].cSteamID = _Leader;
            leaderID = _Leader;
            crewCount = 1;
            isLocked = false;
            password = "";
        }
        public void SetNewLeaderByIndex() // 좌석 순서대로 체크해서 한명을 찾아 새로운 리더로 지정
        {
            foreach (SeatInfo seat in seats)
            {
                if (seat.cSteamID != CSteamID.NonSteamGS) // 좌석에 사람이 있다면
                {
                    // 좌석의 유저가 접속한 상태인지 체크하고, 유저가 없을 시 제거하고 건너뛰기
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(seat.cSteamID);
                    if (uPlayer == null)
                    {
                        seat.cSteamID = CSteamID.NonSteamGS;
                        crewCount--;
                        continue;
                    }
                    leaderID = seat.cSteamID;
                    return;
                }
            }
        }
        public void ChangeSeat(CSteamID _reqPlayerID, byte _seatIndex) // 좌석 변경
        {
            SeatInfo seat = seats.FirstOrDefault(x => x.cSteamID == _reqPlayerID);
            if (seat != default && seats[_seatIndex].cSteamID == CSteamID.NonSteamGS) // 기존 유저 좌석이 확인되고 새로 앉을 좌석이 비어있는게 확인됬다면
            {
                seat.cSteamID = CSteamID.NonSteamGS;
                seats[_seatIndex].cSteamID = _reqPlayerID;
            }
        }
        public void ExileSeat(byte _seatIndex, bool _team) // 좌석에서 유저를 추방
        {
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(seats[_seatIndex].cSteamID);
            ITransportConnection tc = UnturnedPlayer.FromCSteamID(playerInfo.cSteamID).Player.channel.GetOwnerTransportConnection();
            PluginManager.teamInfo.RemovePlayerFromVehicleGroup(seats[_seatIndex].cSteamID, playerInfo, _team);
            VehicleGroupSystem.RefreshUIVehicleGroupAll(tc, playerInfo.vGroupInstanceID, _team);
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleInfo", false);
            UISystem.SendPopUpInfo(tc, "차량그룹에서 추방되었습니다.");
        }
    }
    public class SeatInfo
    {
        public CSteamID cSteamID = CSteamID.NonSteamGS;
        public bool isLocked { get; set; }
    }
}
