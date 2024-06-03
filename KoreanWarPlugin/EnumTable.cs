using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin
{
    public static class EnumTable
    {
        public enum EClassType // 병과 종류
        {
            infantary = 0,
            driver = 1
        }
        public enum EPlayerUIState // 플레이어 UI 상태
        {
            TeamSelect = 0,
            Loadout = 1,
            InGame = 2,
            Death = 3,
            RoundEnd = 4
        }
        public enum ELoadoutType
        {
            primary = 0,
            sight = 1,
            tactical = 2,
            magazine = 3,
            grip = 4,
            secondary = 5,
            explosive_0 = 6,
            explosive_1 = 7,
            equipment = 8,
            utility_0 = 9,
            utility_1 = 10,
        }
        public enum ETireEquip
        {
            I = 0,
            II = 1,
            III = 2,
        }
        public enum ERequestType
        {
            Create = 0,
            Join = 1,
            Switch = 3,
            Exile = 4,
            ChangeVehicle = 5
        }
        public enum EReadyBlockType
        {
            None = 0,
            Loading = 1,
            NotLeader = 2,
            MaxSupply = 3,
            MaxCredit = 4,
            vGroupMinPlayers = 5,
            SpawnPointSelect = 6
        }
        public enum EChatType
        {
            Team = 0,
            All = 1,
            Vehicle = 2
        }
        public enum EMapSize
        {
            Small = 256,
            Medium = 512,
            Large = 1024
        }
        public enum EDeployVehicleState
        {
            Normal = 0,
            Abandon = 1,
            Destroyed = 2
        }
        public enum EObjectiveTeam
        {
            Team_0 = 0,
            Netural = 1,
            Team_1 = 2
        }
        public enum ERoundType
        {
            Annihilation = 0,
            CaptureTheFlag = 1,
            Battle = 2,
            Free = 3, // 자유모드는 항상 맨 뒤에 있게 하기
        }
        public enum EScoreGainType
        {
            EnemyKill = 0,
            ObjectiveCapture = 1,
            ObjectiveNeturalize = 2,
            VehicleDestroy = 3,
            FriendlyRevive = 4,
            FriendlySupply = 5,
            FriendlyDeploy = 6,
        }
    }
}
