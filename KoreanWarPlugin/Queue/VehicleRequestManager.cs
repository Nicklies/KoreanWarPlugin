using KoreanWarPlugin.Info;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Queue
{
    public class VehicleRequestManager // 신규 차량 생성 대기열
    {
        public Queue<VehicleRequest> vehicleRequestQueue = new Queue<VehicleRequest>();
        public VehicleRequest currentVehicleRequest;

        bool isProcessing;
        public void RequestVehicle(CSteamID _cSteamID, byte _vTypeIndex, ushort _vGroupInstanceID, bool _team, EnumTable.ERequestType _type, Action<CSteamID, byte, ushort, bool, EnumTable.ERequestType, bool> callback)
        {
            VehicleRequest newRequest = new VehicleRequest(_cSteamID, _vTypeIndex, _vGroupInstanceID, _team, _type, callback);
            vehicleRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }
        void TryProcessNext()
        {
            if (!isProcessing && vehicleRequestQueue.Count > 0)
            {
                currentVehicleRequest = vehicleRequestQueue.Dequeue();
                isProcessing = true;
                ProcessNewVehicleRequest();
            }
        }
        public void FinishedProcessing(bool _success)
        {
            isProcessing = false;
            currentVehicleRequest.callback(currentVehicleRequest.cSteamID, currentVehicleRequest.index, currentVehicleRequest.vGroupInstanceID, currentVehicleRequest.team, currentVehicleRequest.type, _success);
            TryProcessNext();
        }
        void ProcessNewVehicleRequest()
        {
            bool success = false;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(currentVehicleRequest.cSteamID);
            if (uPlayer != null)
            {
                List<VehicleGroupInfo> vGroupList = currentVehicleRequest.team ? PluginManager.teamInfo.team_0_VehicleGroups : PluginManager.teamInfo.team_1_VehicleGroups;
                VehicleGroupInfo groupInfo = null;
                if (currentVehicleRequest.vGroupInstanceID != ushort.MaxValue) groupInfo = vGroupList.FirstOrDefault(x => x.instanceID == currentVehicleRequest.vGroupInstanceID);
                switch (currentVehicleRequest.type)
                {
                    case EnumTable.ERequestType.Create:
                        List<VehicleTypeInfo> vehicleTypeInfo = currentVehicleRequest.team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_1_VehicleTypes;
                        if (vehicleTypeInfo[currentVehicleRequest.index].presetInfo.vehicleMax == 0) success = true; // 병력 제한이 없다면 바로 통과
                        else
                        {
                            if (vehicleTypeInfo[currentVehicleRequest.index].vehicleCount >= vehicleTypeInfo[currentVehicleRequest.index].presetInfo.vehicleMax) success = false; // 인원을 초과한경우 실패
                            else success = true;
                        }
                        break;
                    case EnumTable.ERequestType.Join:
                        if (groupInfo != default) // 올바른 차량그룹이 존재하는 경우
                        {
                            if (groupInfo.crewCount < groupInfo.maxSeats) success = true; // 좌석이 아직 여유가 있는경우
                        }
                        break;
                    case EnumTable.ERequestType.Switch: // 좌석 변경
                        PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(currentVehicleRequest.cSteamID);
                        if (groupInfo.seats[currentVehicleRequest.index].cSteamID == CSteamID.NonSteamGS && playerInfo.vGroupInstanceID != ushort.MaxValue) success = true; // 좌석에 아무도 없고 차량에서 나가진 상태가 아니라면 성공
                        success = true;
                        break;
                    case EnumTable.ERequestType.Exile: // 추방
                        if (PluginManager.teamInfo.playerInfoList.Keys.FirstOrDefault(x => x == groupInfo.seats[currentVehicleRequest.index].cSteamID) != default) success = true; // 유저가 정상적으로 존재한다면 성공
                        break;
                    case EnumTable.ERequestType.ChangeVehicle: // 차량 교체
                        success = true;
                        break;
                }
            }
            FinishedProcessing(success);
        }
    }
}