﻿using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoreanWarPlugin.Configuration;
using KoreanWarPlugin.Configuration.Preset;
using UnityEngine;

namespace KoreanWarPlugin
{
    public class PluginConfiguration : IRocketPluginConfiguration
    {
        public string LoadMessage { get; set; }
        public WeaponInfoPreset[] weaponInfoPresets { get; set; } // 무기 정보 프리셋
        public MagazineInfoPreset[] magazineInfoPresets { get; set; } // 탄창 정보 프리셋
        public TeamPresetTable[] teamPresets { get; set; } // 팀 정보 프리셋
        public ClassPresetTable[] classPresets { get; set; } // 병과 정보 프리셋
        public PrimaryPresetTable[] primaryPresets { get; set; } // 주무기 장비 프리셋
        public LoadoutTable[] attachmentPresets { get; set; } // 총기 부착물 프리셋
        public SecondaryPresetTable[] secondaryPresets { get; set; } // 부무장 프리셋
        public SecondaryPresetTable[] explosivePresets { get; set; } // 폭팔물 프리셋
        public EquipmentPresetTable[] equipmentPresets { get; set; } // 장구류 프리셋
        public LoadoutTable[] utilityPresets { get; set; } // 특수장비 프리셋
        public VehicleTypePresetTable[] vehicleTypePresets { get; set; } // 차량 종류 프리셋 / 차량 대기란에 이용될 정보
        public VehiclePresetTable[] vehiclePresets { get; set; } // 차량 프리셋
        public MapPreset[] mapPresets { get; set; } // 맵 프리셋
        public GameModePreset[] gameModePresets { get; set; } // 게임모드 프리셋
        public ushort[] levelExpPresets; // 각 레벨별로 레벨업에 필요한 경험치량 프리셋 / 배열 개수만큼 최대 레벨이 정해짐
        public ushort[] reviveItemPresets { get; set; } // 소생 가능 회복 아이템 프리셋
        public ushort[] supplyObject { get; set; } // 보급 오브젝트
        public string[] objectiveEffectGuid { get; set; } // 거점 이펙트 GUID
        public string discordUrl { get; set; } // 디스코드 링크
        public byte teamPresetIndex_0 { get; set; } // 0 팀 프리셋 순서
        public byte teamPresetIndex_1 { get; set; } // 1 팀 프리셋 순서
        public byte respawnTimer { get; set; } // 사망 후 리스폰까지 걸리는 시간
        public byte classInterval { get; set; } // 일정 유저 수 만큼 병과 제한이 상승하는 기준 치
        public byte teamRestrictCount { get; set; } // 유저가 많은 팀에 접속을 제한하는 인원 수 기준
        public Vector3 spawnPos { get; set; } // 대기방 스폰 위치
        public float spawnRot { get; set; } // 대기방 스폰 회전값
        public int supplyCooltime_Inf { get; set; } // 보병 보급 쿭타임
        public int teamChangeDelay { get; set; } // 팀 선택 후 다른 팀으로 변경 가능해질 때까지 걸리는 시간
        public byte freeModeReadyCount { get; set; } // 자유모드 상태에서 게임시작에 필요한 인원 수
        public byte freeModeMapIndex { get; set; } // 자유모드 일시 선택되는 맵의 인덱스
        public void LoadDefaults()
        {
            LoadMessage = "this is KoreanWarPlugin";
            weaponInfoPresets = new WeaponInfoPreset[]
            {
                new WeaponInfoPreset
                {
                    id = 46000,
                    name = "M1 카빈",
                    iconUrl = "https://drive.google.com/uc?id=16DkIZw9nirAr2ZIMJyd3Fg-IlwiA5lQh",
                    isImageLarge = true
                }, // 0. M1카빈
                new WeaponInfoPreset
                {
                    id = 46002,
                    name = "M2 카빈",
                    iconUrl = "https://drive.google.com/uc?id=1BBdJ1vMmSec3FI2woMpLUUuRWCXJW-Vv",
                    isImageLarge = true
                }, // 1. M2카빈
                new WeaponInfoPreset
                {
                    id = 46004,
                    name = "M1 개런드",
                    iconUrl = "https://drive.google.com/uc?id=1RcZhzNdSRAXAoeRZdzG-NhyEUKzkRFnT",
                    isImageLarge = true
                }, // 2. M1개런드
                new WeaponInfoPreset
                {
                    id = 46006,
                    name = "M3 그리스건",
                    iconUrl = "https://drive.google.com/uc?id=1eAJUe8P5yNCDwCZTTRMTbNxszejnhMlg",
                    isImageLarge = true
                }, // 3. M3그리스건
                new WeaponInfoPreset
                {
                    id = 46008,
                    name = "B.A.R",
                    iconUrl = "https://drive.google.com/uc?id=1m_obAklktwePCkXwiKrhCcbpGwNuSd9v",
                    isImageLarge = true
                }, // 4. B.A.R
                new WeaponInfoPreset
                {
                    id = 46010,
                    name = "M1911 콜트",
                    iconUrl = "https://drive.google.com/uc?id=1pJHVRNxBv-jWKbstk_ZpkNZzwvY4j7XV",
                    isImageLarge = false
                }, // 5. M1911콜트
                new WeaponInfoPreset
                {
                    id = 46012,
                    name = "M1903 스프링필드",
                    iconUrl = "https://drive.google.com/uc?id=1HRNQSyR6f-bjd8Y4eO1rp0tAQye8TpD0",
                    isImageLarge = true
                }, // 6. M1908스프링필드
                new WeaponInfoPreset
                {
                    id = 46014,
                    name = "모신난강",
                    iconUrl = "https://drive.google.com/uc?id=1H2NH5f-wLXM1ea-ECHp2-df3Vozgru8v",
                    isImageLarge = true
                }, // 7. 모신난강
                new WeaponInfoPreset
                {
                    id = 46016,
                    name = "PPSH-41",
                    iconUrl = "https://drive.google.com/uc?id=1k8FIBor-lD43zQhyrvEGuUYs1HBV5g4D",
                    isImageLarge = true
                }, // 8. PPSH-41
                new WeaponInfoPreset
                {
                    id = 46019,
                    name = "DP-28",
                    iconUrl = "https://drive.google.com/uc?id=1m9K_piYaW3ncvB1eKV55dC6JACkJb2XU",
                    isImageLarge = true
                }, // 9. DP-28
                new WeaponInfoPreset
                {
                    id = 46021,
                    name = "SVT-40",
                    iconUrl = "https://drive.google.com/uc?id=1tWQSO9gcPAwrsySQD1WdwaLKmwiX0oIw",
                    isImageLarge = true
                }, // 10. SVT-40
                new WeaponInfoPreset
                {
                    id = 46023,
                    name = "TT33",
                    iconUrl = "https://drive.google.com/uc?id=1-oRwP41syw7EwU4fgHCo-O6hxB1lNDtb",
                    isImageLarge = false
                }, // 11. TT33
                new WeaponInfoPreset
                {
                    id = 46025,
                    name = "M9A1 바주카",
                    iconUrl = "https://drive.google.com/uc?id=1ZeM1B0pfurFuJCPtuDwKmIfMENdDwqNX",
                    isImageLarge = false
                }, // 12. M9A1바주카
                new WeaponInfoPreset
                {
                    id = 46027,
                    name = "M20 바주카",
                    iconUrl = "https://drive.google.com/uc?id=1lE8WEYV6X_ECdH5djIdgFwwrNTVHbwe9",
                    isImageLarge = false
                }, // 13. M20바주카
                new WeaponInfoPreset
                {
                    id = 46029,
                    name = "PPS-43",
                    iconUrl = "https://drive.google.com/uc?id=1l7ShnBzah8-uOgD3WCuL5cT6Nqx4bgii",
                    isImageLarge = false
                }, // 14. PPS-43
                new WeaponInfoPreset
                {
                    id = 46031,
                    name = "PTRS-41",
                    iconUrl = "https://drive.google.com/uc?id=1Gvb3VY_5sIj97_VHtMuzY2AEYS2UkdvV",
                    isImageLarge = false
                }, // 15. PTRS=41
            }; // 무기 정보 프리셋
            magazineInfoPresets = new MagazineInfoPreset[]
            {
                new MagazineInfoPreset{ id = 46001, name = "카빈 탄창",iconUrl = ""}, // 0. 카빈 탄창
                new MagazineInfoPreset{ id = 46704, name = "75mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 1. 75mm 철갑탄
                new MagazineInfoPreset{ id = 46705, name = "75mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 2. 75mm 고폭탄
                new MagazineInfoPreset{ id = 46806, name = "30구경 탄통",iconUrl = "https://drive.google.com/uc?id=13Hh-5Iz34YYPgfego4EGMwHiRIaRx6dC"}, // 2. 30구경 탄통
            }; // 탄창 정보 프리셋
            teamPresets = new TeamPresetTable[]
            {
                new TeamPresetTable
                {
                    teamName = "대한민국 육군",
                    teamImageUrl = "https://drive.google.com/uc?id=1DuskqHq8ySyRD03xsOnuDLPLiZiRrJUz",
                    teamIconUrl = "https://drive.google.com/uc?id=1NeiuYPwvBOjBWiMToZl6vlfachTgh-Bm",
                    classList = new ushort[] { 0,1,2,3 },
                    vehicleTypeList = new ushort[] { 0,1,2 },
                    levelPresets = new LevelPreset[]
                    {
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1iHdzq98tM3vgvZZN3aZ5GERU9rOH9xan", name = "이등병" }, // 0. 이등병
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=12shJqP4CecuYUZQI7rdMd-fp3qiYnM6K", name = "일등병" }, // 1. 일등병
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1zFpAALNVh5YVL8xR6hIQAgO6rVnwl9vA", name = "하사" }, // 2. 하사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1Mngp_atjvU3_MtKwxG8JiXmFlEd9_qWP", name = "이등중사" }, // 3. 이등중사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1MN5aU2T3ARo3sxhOtMt8A3UxaRzy_A88", name = "일등중사" }, // 4. 일등중사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=17PKM7AXo7gGCteR1EMlq2v9E6HtFDVDQ", name = "이등상사" }, // 5. 이등상사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1nd1aEMgWOTLDxPZmfNNK_V525vVzysPC", name = "일등상사" }, // 6. 일등상사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=19fXjKFawwpj75L2sDM-Jr4SCOxWkQxa2", name = "특무상사" }, // 7. 특무상사
                    }
                }, // 0.대한민국육군
                new TeamPresetTable
                {
                    teamName = "조선인민군",
                    teamImageUrl = "https://drive.google.com/uc?id=1JZeYOMQorr8aSVwdAaxZJWRvzeK5nnx0",
                    teamIconUrl = "https://drive.google.com/uc?id=1_coLkxN2mvZIeoiYcF5Ke32HkBZiwoKc",
                    classList = new ushort[] { 4,5,6,7 },
                    vehicleTypeList = new ushort[] { 3,4,5 },
                    levelPresets = new LevelPreset[]
                    {
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1vEkMxiI4hmyZEFX3h9mkV-MpeJqEimI8", name = "전사" }, // 0. 전사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1OWJddg66yBRTBfVh6Sv8dn-7aeyR0jgX", name = "하급전사" }, // 1. 하급전사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=15jzieYYEIZi5PUpLAtUXhMXmh9_Legtp", name = "중급전사" }, // 2. 중급전사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1sBH3Z5O08oCd56PhXwksXKgVbqvtHK0H", name = "상급전사" }, // 3. 상급전사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1E2B5jfH00mXYVrvUkj9buDuLHrYiTujB", name = "하사" }, // 4. 하사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1EZ0vHnZclDEuqm7iek3pqFno8q78UbsZ", name = "중사" }, // 5. 중사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=1VFh2KTjwQRX1d1lhLk9jvd9IeI6V0xLB", name = "상사" }, // 6. 상사
                        new LevelPreset{ iconUrl = "https://drive.google.com/uc?id=12oE3HSoKaBKiyWUi50JQ7PRysKCXlXTM", name = "특무상사" }, // 7. 특무상사
                    }
                }, // 1.조선인민군
            };
            classPresets = new ClassPresetTable[]
            {
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "소총병",
                   iconUrl = "https://drive.google.com/uc?id=17IprXj6dEEiQbMa7FkB7nV06TKmj3v4Y",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 0,
                   supplyPoint = 10,
                   isMedic = false,
                   primaryList = new ushort[] { 0, 2 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0, 1 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 2,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 0. 국군 소총병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "의생병",
                   iconUrl = "https://drive.google.com/uc?id=1sDYrs1c5sRF5cUUlooGf9KWiAfP5KgDm",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 4, 7 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = 4,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46502, 46502, 46506, 46506, 46506, 46506, 46506 }
                }, // 1. 국군 의생병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.driver,
                   name = "운전병",
                   iconUrl = "https://drive.google.com/uc?id=1IdV25uFRhmYdsROVX8KM-w8tIcLYPJVe",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 0,
                   supplyPoint = 10,
                   isMedic = false,
                   primaryList = new ushort[] { 6 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = 6,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 2. 국군 운전병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.driver,
                   name = "전차병",
                   iconUrl = "https://drive.google.com/uc?id=1IdV25uFRhmYdsROVX8KM-w8tIcLYPJVe",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 0,
                   supplyPoint = 10,
                   isMedic = false,
                   primaryList = new ushort[] { 4 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = ushort.MaxValue,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 0,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 3. 국군 전차병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "소총병",
                   iconUrl = "https://drive.google.com/uc?id=17IprXj6dEEiQbMa7FkB7nV06TKmj3v4Y",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 0,
                   supplyPoint = 10,
                   isMedic = false,
                   primaryList = new ushort[] { 1, 3 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0, 1 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 1,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 4. 북한군 소총병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "의생병",
                   iconUrl = "https://drive.google.com/uc?id=1sDYrs1c5sRF5cUUlooGf9KWiAfP5KgDm",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 5, 8 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = 5,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46502, 46502,46506, 46506, 46506, 46506, 46506 }
                }, // 5. 북한군 의생병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.driver,
                   name = "운전병",
                   iconUrl = "https://drive.google.com/uc?id=1IdV25uFRhmYdsROVX8KM-w8tIcLYPJVe",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 0,
                   supplyPoint = 10,
                   isMedic = false,
                   primaryList = new ushort[] { 6 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = 6,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 6. 북한군 운전병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.driver,
                   name = "전차병",
                   iconUrl = "https://drive.google.com/uc?id=1IdV25uFRhmYdsROVX8KM-w8tIcLYPJVe",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 0,
                   supplyPoint = 10,
                   isMedic = false,
                   primaryList = new ushort[] { 4 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 0 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = ushort.MaxValue,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 7. 북한군 전차병
            }; // 병과 프리셋
            for (ushort i = 0; i < classPresets.Length; i++) classPresets[i].instanceID = i; // 병과 인스턴스 아이디 부여

            primaryPresets = new PrimaryPresetTable[]
            {
                new PrimaryPresetTable
                {
                    name = "M1908스프링필드",
                    iconUrl = "https://drive.google.com/uc?id=1v7lOn0oNMgfmUu_VaPDvnnru9cMPy3YB",
                    itemID = 46012,
                    supplyCost = 1,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 0 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 0,
                    gripDefaultIndex = ushort.MaxValue
                }, // 0.M1908스프링필드
                new PrimaryPresetTable
                {
                    name = "모신난강",
                    iconUrl = "https://drive.google.com/uc?id=1ZMwza9UbM7pqUAL2t-HR8-lTQLIBO-jd",
                    itemID = 46014,
                    supplyCost = 1,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 1 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 1,
                    gripDefaultIndex = ushort.MaxValue
                }, // 1.모신난강
                new PrimaryPresetTable
                {
                    name = "M1개런드",
                    iconUrl = "https://drive.google.com/uc?id=1kI45O-yjnbo-fdmTTh33EGCykuXjU-3h",
                    itemID = 46004,
                    supplyCost = 4,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 2 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 2,
                    gripDefaultIndex = ushort.MaxValue
                }, // 2.개런드
                new PrimaryPresetTable
                {
                    name = "SVT-40",
                    iconUrl = "https://drive.google.com/uc?id=10iM0kj8y05zHD2x9b2RaoMoEq9UNPdMo",
                    itemID = 46021,
                    supplyCost = 4,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 3 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 3,
                    gripDefaultIndex = ushort.MaxValue
                }, // 3.SVT-40
                new PrimaryPresetTable
                {
                    name = "M3그리스건",
                    iconUrl = "https://drive.google.com/uc?id=1BSIhoRuGLV2hirJg88_iWvXx3W4BHjdu",
                    itemID = 46006,
                    supplyCost = 3,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 4 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 4,
                    gripDefaultIndex = ushort.MaxValue
                }, // 4.M3그리스건
                new PrimaryPresetTable
                {
                    name = "PPS-43",
                    iconUrl = "https://drive.google.com/uc?id=1GznfyjaI-NNy2VWs5C2m-CJlxHlz4FSt",
                    itemID = 46029,
                    supplyCost = 3,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 5 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 5,
                    gripDefaultIndex = ushort.MaxValue
                }, // 5.PPS-43
                new PrimaryPresetTable
                {
                    name = "M1카빈",
                    iconUrl = "https://drive.google.com/uc?id=13a7jEmmVFGxRSJwWTiRtkBou_MTYSHPU",
                    itemID = 46000,
                    supplyCost = 2,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 6, 7 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 6,
                    gripDefaultIndex = ushort.MaxValue
                }, // 6.M1카빈
                new PrimaryPresetTable
                {
                    name = "M2카빈",
                    iconUrl = "https://drive.google.com/uc?id=1LjFG_Gy6yJ0RC_szx5HOPfC6URZAfam5",
                    itemID = 46002,
                    supplyCost = 4,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 6, 7 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 6,
                    gripDefaultIndex = ushort.MaxValue
                }, // 7.M2카빈
                new PrimaryPresetTable
                {
                    name = "PPSH-41",
                    iconUrl = "https://drive.google.com/uc?id=1TJrybyZBYwJft7oa-QK6uv2bQbL0q_he",
                    itemID = 46016,
                    supplyCost = 4,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 8, 9 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 8,
                    gripDefaultIndex = ushort.MaxValue
                }, // 8.PPSH-41
                new PrimaryPresetTable
                {
                    name = "M1918 BAR",
                    iconUrl = "https://drive.google.com/uc?id=1iNHle3lCby_051iYJuaBqbRCJA7QPrAG",
                    itemID = 46008,
                    supplyCost = 4,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 10 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 10,
                    gripDefaultIndex = ushort.MaxValue
                }, // 9.M1918BAR
                new PrimaryPresetTable
                {
                    name = "DP-28",
                    iconUrl = "https://drive.google.com/uc?id=1XFhxTkJgPftbgeWPiXyiDaLUCttLfOSL",
                    itemID = 46019,
                    supplyCost = 4,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 11 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 11,
                    gripDefaultIndex = ushort.MaxValue
                }, // 10.DP-28
            }; // 주무장 프리셋
            attachmentPresets = new LoadoutTable[]
            {
                new LoadoutTable
                {
                    name = "스프링필드 클립",
                    iconUrl = "https://drive.google.com/uc?id=1SnZjdOk4qwhcD1Kgdg7UduY4Uh-Z177A",
                    itemID = 46013,
                    supplyCost = 0,
                    amount = 0, // 장구류 구분없이 똑같이 줄거면 이거 사용
                    amount_equipment = new byte[]{ 8, 10, 12 } // 장구류에 따른 탄창 개수
                }, // 0. M1908스프링필드 클립
                new LoadoutTable
                {
                    name = "모신난강 클립",
                    iconUrl = "https://drive.google.com/uc?id=1PzsG3c1ToYio9LxMVEEjDWTZyz91lmMg",
                    itemID = 46015,
                    supplyCost = 0,
                    amount = 0,
                    amount_equipment = new byte[]{ 8, 10, 12 }
                }, // 1. 모신난강 탄창
                new LoadoutTable
                {
                    name = "개런드 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1LP5z_VIarNmpMHABTn9qRpGctm9Su6yi",
                    itemID = 46005,
                    supplyCost = 0,
                    amount = 0,
                    amount_equipment = new byte[]{ 4, 6, 7 }
                }, // 2. 개런드 탄창
                new LoadoutTable
                {
                    name = "SVT-40 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1dubqpzpFMAsma9tONOZEr9q4_Yc5I6B5",
                    itemID = 46022,
                    supplyCost = 0,
                    amount = 0,
                    amount_equipment = new byte[]{ 4, 5, 6 }
                }, // 3. SVT-40 탄창
                new LoadoutTable
                {
                    name = "그리스건 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1w76U8EhmNskiCZhu3_qncFX506Hy-IdP",
                    itemID = 46007,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 3, 4, 5 }
                }, // 4. M3그리스건 탄창
                new LoadoutTable
                {
                    name = "PPS-43 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1sJGnfFWRT2NayQq91kmV302HKVDeX7mr",
                    itemID = 46030,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 3, 4, 5 }
                }, // 5. PPS-43 탄창
                new LoadoutTable
                {
                    name = "카빈 탄창",
                    iconUrl = "https://drive.google.com/uc?id=12nmrQwKmwGymTFwShQXylhdfG5RyOKqR",
                    itemID = 46001,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 4, 6, 8 }
                }, // 6. 카빈 탄창
                new LoadoutTable
                {
                    name = "카빈 확장 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1jGG6H-W8B1R1xNXlub-OzI51DqtO7bMD",
                    itemID = 46003,
                    supplyCost = 2,
                    amount_equipment = new byte[]{ 2, 3, 4 }
                }, // 7. 카빈 확장 탄창
                new LoadoutTable
                {
                    name = "PPSH 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1MO__RwmaWM8X8dcYXiPE9B9cBIkcKihQ",
                    itemID = 46017,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 4, 5, 6 }
                }, // 8. PPSH 탄창
                new LoadoutTable
                {
                    name = "PPSH 드럼 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1L2blcWSnOH2JWUYWG4uDzmpGegRqk1gG",
                    itemID = 46018,
                    supplyCost = 2,
                    amount_equipment = new byte[]{ 2, 3, 4 }
                }, // 9. PPSH 드럼 탄창
                new LoadoutTable
                {
                    name = "BAR 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1Ujb-sLBm_AewRJHt4IaAX5z4qcgopey8",
                    itemID = 46009,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 3, 5, 7 }
                }, // 10. BAR 탄창
                new LoadoutTable
                {
                    name = "DP-28 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1IRspuTRfiI1fA8HbIzqDdnT4KqPu0Ppq",
                    itemID = 46020,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 1, 2, 3 }
                }, // 11. DP-28 탄창
            }; // 총기 부착물 프리셋
            secondaryPresets = new SecondaryPresetTable[]
            {
                new SecondaryPresetTable
                {
                    name = "M1911콜트",
                    iconUrl = "https://drive.google.com/uc?id=1BWHRtNAKB4YAXbLActRKH_RHteBHR_3t",
                    itemID = 46010,
                    supplyCost = 1,
                    amount = 1,
                    magazineItemID = 46011,
                    magazineAmount = 0, // 장구류 구분없이 탄창 똑같이 줄거면 이거 사용
                    amount_equipment = new byte[]{ 2, 3, 4 } // 장구류에 따른 탄창 개수
                }, // 0. M1911콜트
            }; // 부무장 프리셋
            explosivePresets = new SecondaryPresetTable[]
            {
                new SecondaryPresetTable
                {
                    name = "F1 슈류탄",
                    iconUrl = "https://drive.google.com/uc?id=1mrElNeWellpyefWSXgoOSSlNThu7YcP8",
                    itemID = 46400,
                    amount = 1,
                    supplyCost = 1,
                    isDuplicatable = true
                }, // 0. F1 슈류탄
                new SecondaryPresetTable
                {
                    name = "M9A1 바주카",
                    iconUrl = "https://drive.google.com/uc?id=1UfQq1OZZ52f-5sG1jCNlq-2hHLTtZJgY",
                    itemID = 46025,
                    amount = 1,
                    supplyCost = 4,
                    magazineItemID = 46026,
                    magazineAmount = 0,
                    amount_equipment = new byte[] { 1, 2, 3 },
                    isDuplicatable = false
                }, // 1. M9A1바주카
            };
            equipmentPresets = new EquipmentPresetTable[]
            {
                new EquipmentPresetTable
                {
                    clothPresetList = new ClothPresetTable[]
                    {
                        new ClothPresetTable
                        {
                            name = "비무장",
                            iconUrl = "https://drive.google.com/uc?id=1Q4i5y3qDo7ROTyWd-o4VxKqDsyv-Qr32",
                            supplyCost = 0,
                            equipment_Head = new ushort[]{ 46108 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{  },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46303 },
                            equipment_Pant = new ushort[]{  },
                        }, // 비무장
                        new ClothPresetTable
                        {
                            name = "경무장",
                            iconUrl = "https://drive.google.com/uc?id=1IhLPr6N2lox2qSimdFNyNObIPlRXrw59",
                            supplyCost = 2,
                            equipment_Head = new ushort[]{ 46108 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46200 },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46303 },
                            equipment_Pant = new ushort[]{  },
                        },
                        new ClothPresetTable
                        {
                            name = "중무장",
                            iconUrl = "https://drive.google.com/uc?id=1lxXg35MPZQGxSuzhHEUMtD1gGCcLCAza",
                            supplyCost = 4,
                            equipment_Head = new ushort[]{ 46100, 46111, 46112, 46113, 46114, 46115, 46116, 46117 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46201 },
                            equipment_Backpack = new ushort[]{ 46600 },
                            equipment_shirt = new ushort[]{ 46303 },
                            equipment_Pant = new ushort[]{  },
                        }
                    }
                }, // 0. 한국군 소총병 장구류
                new EquipmentPresetTable
                {
                    clothPresetList = new ClothPresetTable[]
                    {
                        new ClothPresetTable
                        {
                            name = "비무장",
                            iconUrl = "https://drive.google.com/uc?id=1Q4i5y3qDo7ROTyWd-o4VxKqDsyv-Qr32",
                            supplyCost = 0,
                            equipment_Head = new ushort[]{ 46104, 46104, 46104, 46104, 46109, 46109, 46109, 46109 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{  },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46311, 46312, 46313, 46314, 46315, 46316, 46317, 46318 },
                            equipment_Pant = new ushort[]{  },
                        }, // 비무장
                        new ClothPresetTable
                        {
                            name = "경무장",
                            iconUrl = "https://drive.google.com/uc?id=1IhLPr6N2lox2qSimdFNyNObIPlRXrw59",
                            supplyCost = 2,
                            equipment_Head = new ushort[]{ 46104, 46104, 46104, 46104, 46109, 46109, 46109, 46109 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46210 },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46311, 46312, 46313, 46314, 46315, 46316, 46317, 46318 },
                            equipment_Pant = new ushort[]{  },
                        },
                        new ClothPresetTable
                        {
                            name = "중무장",
                            iconUrl = "https://drive.google.com/uc?id=1lxXg35MPZQGxSuzhHEUMtD1gGCcLCAza",
                            supplyCost = 4,
                            equipment_Head = new ushort[]{ 46106 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46211 },
                            equipment_Backpack = new ushort[]{ 46601 },
                            equipment_shirt = new ushort[]{ 46311, 46312, 46313, 46314, 46315, 46316, 46317, 46318 },
                            equipment_Pant = new ushort[]{  },
                        }
                    }
                }, // 1. 북한군 소총병 장구류
            }; // 장구류 프리셋
            utilityPresets = new LoadoutTable[]
            {
                new LoadoutTable
                {
                    name = "망원경",
                    iconUrl = "",
                    itemID = 333,
                    amount = 1,
                    supplyCost = 1,
                }, // 0. 망원경
            };
            vehicleTypePresets = new VehicleTypePresetTable[]
            {
                new VehicleTypePresetTable
                {
                    name = "수송차량",
                    iconUrl = "https://drive.google.com/uc?id=1lP1gm_r2Br-wb-OmO0HmMuY9wAtKarFs",
                    levelLimit = 0,
                    timer = 0,
                    vehicleMax = 5,
                    playerMinCount = 0,
                    crewMinCount = 1,
                    classIndex = 0,
                    vehicleList = new ushort[]{ 6 },
                    respawnTime = 30,
                    abandonTime = 600,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = false
                }, // 0. 한국 수송차량
                new VehicleTypePresetTable
                {
                    name = "경전차",
                    iconUrl = "https://drive.google.com/uc?id=1v2FoM2sP5Zo4LPLrhLIpPxNvJlPjlOsy",
                    levelLimit = 0,
                    timer = 15,
                    vehicleMax = 3,
                    playerMinCount = 0,
                    crewMinCount = 1,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 0 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 1. 한국 경전차
                new VehicleTypePresetTable
                {
                    name = "중전차",
                    iconUrl = "https://drive.google.com/uc?id=1_L86f8Z__AdHiVmwQ0UN6-Rb0XABO0qi",
                    levelLimit = 3,
                    timer = 25,
                    vehicleMax = 2,
                    playerMinCount = 0,
                    crewMinCount = 1,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 1,2 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 2. 한국 중전차
                new VehicleTypePresetTable
                {
                    name = "수송차량",
                    iconUrl = "https://drive.google.com/uc?id=1lP1gm_r2Br-wb-OmO0HmMuY9wAtKarFs",
                    levelLimit = 0,
                    timer = 0,
                    vehicleMax = 5,
                    playerMinCount = 0,
                    crewMinCount = 6,
                    classIndex = 0,
                    vehicleList = new ushort[]{ 7 },
                    respawnTime = 30,
                    abandonTime = 600,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = false
                }, // 3. 북한 수송차량
                new VehicleTypePresetTable
                {
                    name = "경전차",
                    iconUrl = "https://drive.google.com/uc?id=1v2FoM2sP5Zo4LPLrhLIpPxNvJlPjlOsy",
                    levelLimit = 0,
                    timer = 0,
                    vehicleMax = 3,
                    playerMinCount = 2,
                    crewMinCount = 1,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 3 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 4. 북한 경전차
                new VehicleTypePresetTable
                {
                    name = "중전차",
                    iconUrl = "https://drive.google.com/uc?id=1_L86f8Z__AdHiVmwQ0UN6-Rb0XABO0qi",
                    levelLimit = 3,
                    timer = 0,
                    vehicleMax = 2,
                    playerMinCount = 12,
                    crewMinCount = 1,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 4,5 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                } // 5. 북한 중전차
            };
            for (ushort i = 0; i < vehicleTypePresets.Length; i++)
            {
                vehicleTypePresets[i].instanceID = i;
            }
            vehiclePresets = new VehiclePresetTable[]
            {
                new VehiclePresetTable
                {
                    name = "M5A1 스튜어트",
                    iconUrl = "https://drive.google.com/uc?id=116sTAsTUesjRW7pmm4mTrc0atTNJtyX9",
                    itemID = 4620,
                    creditCost = 5,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                    ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46700 ,amount = 20 },
                        new VAmmoPresetTable{ itemID = 46701 ,amount = 20 },
                        new VAmmoPresetTable{ itemID = 46806 ,amount = 2 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                     supplyCooltime = 20
                }, // 0. M5A1 스튜어트
                new VehiclePresetTable
                {
                    name = "M4A3 셔먼",
                    iconUrl = "https://drive.google.com/uc?id=1N-hsEzlowK5Gvw_yoTovqsS7pcnGDuLJ",
                    itemID = 4600,
                    creditCost = 10,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                    ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46704 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46705 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46806 ,amount = 2 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                    supplyCooltime = 20
                }, // 1. M4A3 셔먼
                new VehiclePresetTable
                {
                    name = "M4A3E8 셔먼",
                    iconUrl = "https://drive.google.com/uc?id=1NBi7sXqUYyb20lyAh_CfBsWXhQYzcjMR",
                    itemID = 4604,
                    creditCost = 15,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "지휘관석",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                    ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46706 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46707 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46806 ,amount = 2 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                    supplyCooltime = 20
                }, // 2. M4A3E8 셔먼
                new VehiclePresetTable
                {
                    name = "T-26",
                    iconUrl = "https://drive.google.com/uc?id=1gwhkbamRDIS7wKbbwz8RJlWCl1r9vjq6",
                    itemID = 4622,
                    creditCost = 5,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                    ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46710 ,amount = 20 },
                        new VAmmoPresetTable{ itemID = 46711 ,amount = 20 },
                        new VAmmoPresetTable{ itemID = 46727 ,amount = 5 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                     supplyCooltime = 60
                }, // 3. T-26
                new VehiclePresetTable
                {
                    name = "T-34/76",
                    iconUrl = "https://drive.google.com/uc?id=1XHbEEofNYQyRsPNs0vwOeZ-UgOJGoiY3",
                    itemID = 4605,
                    creditCost = 10,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                    ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46712 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46713 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46727 ,amount = 5 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                     supplyCooltime = 60
                }, // 4. T-34/76
                new VehiclePresetTable
                {
                    name = "T-34/85",
                    iconUrl = "https://drive.google.com/uc?id=1DdmAoiJd5_hd2P60XywqDGuawQu7-rfc",
                    itemID = 4606,
                    creditCost = 15,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                     ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46714 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46715 ,amount = 10 },
                        new VAmmoPresetTable{ itemID = 46727 ,amount = 5 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                     supplyCooltime = 60
                }, // 5. T-34/85
                new VehiclePresetTable
                {
                    name = "CCKW",
                    iconUrl = "https://drive.google.com/uc?id=14H0ODhfh8rtcmCPlit7GoBShUVYDwtx9",
                    itemID = 4602,
                    creditCost = 1,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                    },
                     ammos = new VAmmoPresetTable[] {  },
                     isDeployable = true,
                     isSupplyable = false,
                     supplyCooltime = 60
                }, // 6. CCKW (수송)
                new VehiclePresetTable
                {
                    name = "ZIS-151",
                    iconUrl = "https://drive.google.com/uc?id=1ACo3cygf-tvPWf18IrABOq0PpCUTx1VF",
                    itemID = 4608,
                    creditCost = 1,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                    },
                     ammos = new VAmmoPresetTable[] {  },
                     isDeployable = true,
                     isSupplyable = false,
                     supplyCooltime = 60
                }, // 7. ZIS-151 (수송)
            };
            mapPresets = new MapPreset[]
            {
                new MapPreset
                {
                    name = "테스트맵",
                    mapImageUrl = "https://drive.google.com/uc?id=12dOZgfoHgWFbTDPMdaz4M328_AffJway",
                    mapIconUrl = "https://drive.google.com/uc?id=12pvu_DMBcZMcZpsIPIF5VdhzzKksD0jV",
                    mapSize = EnumTable.EMapSize.Small,
                    // 0 = 자유, 1 = 섬멸전, 2 = 깃발점령전, 3 = 공방전
                    playerCount = 24,
                    roundType = new byte[] { 2 ,3 },
                    mapPositon = new Vector3(-1984,30,-1984),
                    ObjectivePresets = new ObjectivePreset[]
                    {
                        new ObjectivePreset
                        {
                            position = new Vector3(-1856,30,-1856), rotation = Quaternion.Euler(0,0,0), size = new Vector3(10,10,10),
                            team_0_MarkerPos = new Vector3(-1920, 30, -1920),
                            team_1_MarkerPos = new Vector3(-1792,30,-1792),
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(-1930,30,-1920), rotation = 45 }, // 1팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(-1920,30,-1930), rotation = 45 }, // 1팀 A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(-1930,30,-1930), rotation = 45 }, // 1팀 A 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(-1782,30,-1792), rotation = 135 }, // 2팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(-1792,30,-1782), rotation = 135 }, // 2팀 A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(-1782,30,-1782), rotation = 135 }, // 2팀 A 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(-1920,30,-1920), rotation = 0, size = new Vector3(2f,10f,2f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(-1792,30,-1792), rotation = 0, size = new Vector3(2f,10f,2f) }
                        }, // A 거점
                    },
                    spawnPos_0 = new SpawnPreset{ position = new Vector3(-1979,30,-1979), rotation = 45 }, // 0 팀 기지 스폰
                    spawnPos_1 = new SpawnPreset{ position = new Vector3(-1733,30,-1733), rotation = 225 }, // 1 팀 기지 스폰
                    baseRestrict_0 = new RestrictPreset{position = new Vector3(-1984,30,-1984), rotation = 0, size = new Vector3(96f,20f,96f)},
                    baseRestrict_1 = new RestrictPreset{position = new Vector3(-1728,30,-1728), rotation = 0, size = new Vector3(96f,20f,96f)},
                    vehicleSpawnPos_0 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(-1964,30,-1974), rotation = 0 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1954,30,-1974), rotation = 0 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1944,30,-1974), rotation = 0 }, // 3 번 차량 스폰 위치
                    }, // 0 팀 차랑 스폰 위치
                    vehicleSpawnPos_1 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(-1748,30,-1738), rotation = 180 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1758,30,-1738), rotation = 180 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1768,30,-1738), rotation = 180 }, // 3 번 차량 스폰 위치
                    }, // 1 팀 차랑 스폰 위치
                }, // 0. 무명고지 맵
            }; // 맵 프리셋
            gameModePresets = new GameModePreset[]
            {
                new GameModePreset
                {
                    name = "섬멸전",
                    playerCount = 24,
                    description = "적을 사살하여 상대의 점수를 낮춰 승리하는것이 목표",
                    iconUrl = "https://drive.google.com/uc?id=10MK6wcTRCGaDSlkNfQTjJ5SgxGEOJDj1",
                    maps = new byte[] { 0 }
                }, // 0. 섬멸전
                new GameModePreset
                {
                    name = "깃발점령전",
                    playerCount = 24,
                    description = "적을 사살하거나 거점을 점령하고 유지해 상대의 점수를 낯춰 승리하는것이 목표",
                    iconUrl = "https://drive.google.com/uc?id=14ZolrjQNQSJ8b1eVqOfPgle3sXoU0WK9",
                    maps = new byte[] { 0 }
                }, // 1. 깃발점령전
                new GameModePreset
                {
                    name = "공방전",
                    playerCount = 24,
                    description = "공격 혹은 방어팀이 되어 각자 임무를 달성하는것이 목표",
                    iconUrl = "https://drive.google.com/uc?id=1dzB67DeAjJe_y04eI5Gav5xNnX8qG9M6",
                    maps = new byte[] { 0 }
                }, // 2. 공방전
            };
            levelExpPresets = new ushort[]
            {
                200, // 0 => 1 레벨
                400, // 1 => 2
                800, // 2 => 3
                1200, // 3 => 4
                1800, // 4 => 5
                2400, // 5 => 6
                3000, // 6 => 7
            }; // 레벨 프리셋
            reviveItemPresets = new ushort[]
            {
                46502
            };
            supplyObject = new ushort[]
            {
                745,
                4700
            }; // 보급 오브젝트
            objectiveEffectGuid = new string[]
            {
                "dc3ddb05d89945088180c83bacaad4df",
                "e20d2fd427ea4c9da2945ee7b4e347d1",
                "30e33f96806b4f908501eecfd5b510d3",
                "8cc568f03f0d419ba9085fcc8686854b",
                "c436b8f3380d4bc4b4447cd12556a854"
            }; // 거점 이펙트 GUID
            discordUrl = "https://discord.gg/kYRnznXS";
            teamPresetIndex_0 = 0;
            teamPresetIndex_1 = 1;
            respawnTimer = 5;
            classInterval = 24;
            teamRestrictCount = 2;
            spawnPos = new Vector3(1972.71f, 30.16f, 1974.63f);
            supplyCooltime_Inf = 30;
            spawnRot = 180f;
            teamChangeDelay = 0;
            freeModeReadyCount = 2;
            freeModeMapIndex = 0;
        }
    }
}