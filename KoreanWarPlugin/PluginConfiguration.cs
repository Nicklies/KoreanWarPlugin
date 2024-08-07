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
    // 플러그인에서 제공되는 콘피그 시스템을 이용해 데이터 저장 중
    // 향후 관계형 데이터베이스를 이용해 데이터 저장 방식 변경할 예정
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
        public ushort[] supplyBarriacde { get; set; } // 보급 바리케이드
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
        // 거점 점령관련 데이터
        public float baseCaptureSpeed { get; set; } // 초당 기본 거점 점령 속도
        public float thresholdCaptureSpeed { get; set; } // 초당 기본 거점 점령 속도 임계점
        public int maxCapturePlayerCount { get; set; } // 점령에 영향을 줄 수 있는 최대 유저 수
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
                }, // M1카빈
                new WeaponInfoPreset
                {
                    id = 46002,
                    name = "M2 카빈",
                    iconUrl = "https://drive.google.com/uc?id=1BBdJ1vMmSec3FI2woMpLUUuRWCXJW-Vv",
                    isImageLarge = true
                }, // M2카빈
                new WeaponInfoPreset
                {
                    id = 46004,
                    name = "M1 개런드",
                    iconUrl = "https://drive.google.com/uc?id=1RcZhzNdSRAXAoeRZdzG-NhyEUKzkRFnT",
                    isImageLarge = true
                }, // M1개런드
                new WeaponInfoPreset
                {
                    id = 46006,
                    name = "M3 그리스건",
                    iconUrl = "https://drive.google.com/uc?id=1eAJUe8P5yNCDwCZTTRMTbNxszejnhMlg",
                    isImageLarge = true
                }, // M3그리스건
                new WeaponInfoPreset
                {
                    id = 46008,
                    name = "B.A.R",
                    iconUrl = "https://drive.google.com/uc?id=1m_obAklktwePCkXwiKrhCcbpGwNuSd9v",
                    isImageLarge = true
                }, // B.A.R
                new WeaponInfoPreset
                {
                    id = 46010,
                    name = "M1911 콜트",
                    iconUrl = "https://drive.google.com/uc?id=1pJHVRNxBv-jWKbstk_ZpkNZzwvY4j7XV",
                    isImageLarge = false
                }, // M1911콜트
                new WeaponInfoPreset
                {
                    id = 46012,
                    name = "M1903 스프링필드",
                    iconUrl = "https://drive.google.com/uc?id=1HRNQSyR6f-bjd8Y4eO1rp0tAQye8TpD0",
                    isImageLarge = true
                }, // M1908스프링필드
                new WeaponInfoPreset
                {
                    id = 46014,
                    name = "모신나강",
                    iconUrl = "https://drive.google.com/uc?id=1H2NH5f-wLXM1ea-ECHp2-df3Vozgru8v",
                    isImageLarge = true
                }, // 모신나강
                new WeaponInfoPreset
                {
                    id = 46016,
                    name = "PPSH-41",
                    iconUrl = "https://drive.google.com/uc?id=1k8FIBor-lD43zQhyrvEGuUYs1HBV5g4D",
                    isImageLarge = true
                }, // PPSH-41
                new WeaponInfoPreset
                {
                    id = 46019,
                    name = "DP-28",
                    iconUrl = "https://drive.google.com/uc?id=1m9K_piYaW3ncvB1eKV55dC6JACkJb2XU",
                    isImageLarge = true
                }, // DP-28
                new WeaponInfoPreset
                {
                    id = 46021,
                    name = "SVT-40",
                    iconUrl = "https://drive.google.com/uc?id=1tWQSO9gcPAwrsySQD1WdwaLKmwiX0oIw",
                    isImageLarge = true
                }, // SVT-40
                new WeaponInfoPreset
                {
                    id = 46023,
                    name = "TT33",
                    iconUrl = "https://drive.google.com/uc?id=1-oRwP41syw7EwU4fgHCo-O6hxB1lNDtb",
                    isImageLarge = false
                }, // TT33
                new WeaponInfoPreset
                {
                    id = 46025,
                    name = "M9A1 바주카",
                    iconUrl = "https://drive.google.com/uc?id=1ZeM1B0pfurFuJCPtuDwKmIfMENdDwqNX",
                    isImageLarge = false
                }, // M9A1바주카
                new WeaponInfoPreset
                {
                    id = 46027,
                    name = "M20 바주카",
                    iconUrl = "https://drive.google.com/uc?id=1lE8WEYV6X_ECdH5djIdgFwwrNTVHbwe9",
                    isImageLarge = false
                }, // M20바주카
                new WeaponInfoPreset
                {
                    id = 46029,
                    name = "PPS-43",
                    iconUrl = "https://drive.google.com/uc?id=1l7ShnBzah8-uOgD3WCuL5cT6Nqx4bgii",
                    isImageLarge = true
                }, // PPS-43
                new WeaponInfoPreset
                {
                    id = 46031,
                    name = "PTRS-41",
                    iconUrl = "https://drive.google.com/uc?id=1Gvb3VY_5sIj97_VHtMuzY2AEYS2UkdvV",
                    isImageLarge = false
                }, // PTRS=41
                new WeaponInfoPreset
                {
                    id = 46718,
                    name = "M1919 브라우닝",
                    iconUrl = "https://drive.google.com/uc?id=1UNGTpGz1fUzUyFjBcaJfDflzrSMz0a-w",
                    isImageLarge = true
                }, // M1919브라우닝
                new WeaponInfoPreset
                {
                    id = 46719,
                    name = "M2 브라우닝",
                    iconUrl = "https://drive.google.com/uc?id=1UNGTpGz1fUzUyFjBcaJfDflzrSMz0a-w",
                    isImageLarge = true
                }, // M2브라우닝
                new WeaponInfoPreset
                {
                    id = 46720,
                    name = "맥심기관총",
                    iconUrl = "https://drive.google.com/uc?id=1UNGTpGz1fUzUyFjBcaJfDflzrSMz0a-w",
                    isImageLarge = true
                }, // 맥심기관총
                new WeaponInfoPreset
                {
                    id = 46721,
                    name = "DHSK",
                    iconUrl = "https://drive.google.com/uc?id=1UNGTpGz1fUzUyFjBcaJfDflzrSMz0a-w",
                    isImageLarge = true
                }, // DHSK
                new WeaponInfoPreset
                {
                    id = 46722,
                    name = "75mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // M4A3주포
                new WeaponInfoPreset
                {
                    id = 46723,
                    name = "76mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // M4A3E8주포
                new WeaponInfoPreset
                {
                    id = 46725,
                    name = "76mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // T34/76주포
                new WeaponInfoPreset
                {
                    id = 46726,
                    name = "DT-29 공축기관총",
                    iconUrl = "https://drive.google.com/uc?id=1UNGTpGz1fUzUyFjBcaJfDflzrSMz0a-w",
                    isImageLarge = true
                }, // DT-29 공축기관총
                new WeaponInfoPreset
                {
                    id = 46728,
                    name = "85mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // T34/85 주포
                new WeaponInfoPreset
                {
                    id = 46729,
                    name = "90mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // M26퍼싱 주포
                new WeaponInfoPreset
                {
                    id = 46730,
                    name = "37mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // 그레이하운드 주포
                new WeaponInfoPreset
                {
                    id = 46731,
                    name = "45mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // BA-11 주포
                new WeaponInfoPreset
                {
                    id = 46733,
                    name = "75mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // M24채피 주포
                new WeaponInfoPreset
                {
                    id = 46734,
                    name = "122mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // IS-2 주포
                new WeaponInfoPreset
                {
                    id = 46737,
                    name = "57mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // M1대전차포 주포
                new WeaponInfoPreset
                {
                    id = 46738,
                    name = "76mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // M5대전차포 주포
                new WeaponInfoPreset
                {
                    id = 46739,
                    name = "45mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // M1942대전차포 주포
                new WeaponInfoPreset
                {
                    id = 46740,
                    name = "76mm 주포",
                    iconUrl = "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm",
                    isImageLarge = true
                }, // ZIS-3대전차포 주포
                new WeaponInfoPreset
                {
                    id = 46041,
                    name = "M1A1톰슨",
                    iconUrl = "https://drive.google.com/uc?id=1uRVjTzDCmBshUXyKN7YXVM0f80GDEWcd",
                    isImageLarge = true
                }, // M1A1톰슨
                new WeaponInfoPreset
                {
                    id = 46044,
                    name = "SKS",
                    iconUrl = "https://drive.google.com/uc?id=17PG2C9bbw4u1T1jPY4RA9zSycDrn10-5",
                    isImageLarge = true
                }, // SKS
            }; // 무기 정보 프리셋
            magazineInfoPresets = new MagazineInfoPreset[]
            {
                new MagazineInfoPreset{ id = 46001, name = "카빈 탄창",iconUrl = ""}, // 카빈 탄창
                new MagazineInfoPreset{ id = 46003, name = "카빈 확장 탄창",iconUrl = ""}, // 카빈 확장 탄창
                new MagazineInfoPreset{ id = 46005, name = "개런드 클립",iconUrl = ""}, // 개런드 클립
                new MagazineInfoPreset{ id = 46007, name = "그리스건 탄창",iconUrl = ""}, // 그리스건 탄창
                new MagazineInfoPreset{ id = 46009, name = "BAR 탄창",iconUrl = ""}, // BAR 탄창
                new MagazineInfoPreset{ id = 46011, name = "M1911 탄창",iconUrl = ""}, // M1911 탄창
                new MagazineInfoPreset{ id = 46013, name = "스프링필드 클립",iconUrl = ""}, // 스프링필드 클립
                new MagazineInfoPreset{ id = 46015, name = "모신나강 클립",iconUrl = ""}, // 모신나강 클립
                new MagazineInfoPreset{ id = 46017, name = "PPSH 탄창",iconUrl = ""}, // PPSH 탄창
                new MagazineInfoPreset{ id = 46018, name = "PPSH 원형 탄창",iconUrl = ""}, // PPSH 원형 탄창
                new MagazineInfoPreset{ id = 46020, name = "DP-28 탄창",iconUrl = ""}, // DP-28 탄창
                new MagazineInfoPreset{ id = 46022, name = "SVT-40 탄창",iconUrl = ""}, // SVT-40 탄창
                new MagazineInfoPreset{ id = 46024, name = "TT-33 탄창",iconUrl = ""}, // TT-33 탄창
                new MagazineInfoPreset{ id = 46026, name = "M9A1 발사체",iconUrl = ""}, // M9A1 발사체
                new MagazineInfoPreset{ id = 46028, name = "M20 발사체",iconUrl = ""}, // M20 발사체
                new MagazineInfoPreset{ id = 46030, name = "PPS-43 탄창",iconUrl = ""}, // PPS-43 탄창
                new MagazineInfoPreset{ id = 46032, name = "PTRS-41 탄창",iconUrl = ""}, // PTRS-41 탄창
                new MagazineInfoPreset{ id = 46042, name = "톰슨 탄창",iconUrl = ""}, // 톰슨 탄창
                new MagazineInfoPreset{ id = 46043, name = "톰슨 드럼 탄창",iconUrl = ""}, // 톰슨 드럼 탄창
                new MagazineInfoPreset{ id = 46045, name = "SKS 클립",iconUrl = ""}, // SKS 클립
                /// 차량용 탄창
                new MagazineInfoPreset{ id = 46806, name = "30구경 탄통",iconUrl = "https://drive.google.com/uc?id=13Hh-5Iz34YYPgfego4EGMwHiRIaRx6dC"}, // 30구경 탄통
                new MagazineInfoPreset{ id = 46808, name = "50구경 탄통",iconUrl = "https://drive.google.com/uc?id=13Hh-5Iz34YYPgfego4EGMwHiRIaRx6dC"}, // 50구경 탄통
                new MagazineInfoPreset{ id = 46727, name = "DT-29 탄창",iconUrl = "https://drive.google.com/uc?id=13Hh-5Iz34YYPgfego4EGMwHiRIaRx6dC"}, // DT-29 탄창
                new MagazineInfoPreset{ id = 46816, name = "중기관총 탄통",iconUrl = "https://drive.google.com/uc?id=13Hh-5Iz34YYPgfego4EGMwHiRIaRx6dC"}, // 중기관총 탄통
                new MagazineInfoPreset{ id = 46700, name = "37mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 37mm 철갑탄
                new MagazineInfoPreset{ id = 46701, name = "37mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 37mm 고폭탄
                new MagazineInfoPreset{ id = 46702, name = "57mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 57mm 철갑탄
                new MagazineInfoPreset{ id = 46703, name = "57mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 57mm 고폭탄
                new MagazineInfoPreset{ id = 46704, name = "75mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 75mm 철갑탄
                new MagazineInfoPreset{ id = 46705, name = "75mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 75mm 고폭탄
                new MagazineInfoPreset{ id = 46706, name = "76mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 76mm 철갑탄
                new MagazineInfoPreset{ id = 46707, name = "76mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 76mm 고폭탄
                new MagazineInfoPreset{ id = 46708, name = "90mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 90mm 철갑탄
                new MagazineInfoPreset{ id = 46709, name = "90mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 90mm 고폭탄
                new MagazineInfoPreset{ id = 46710, name = "45mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 45mm 철갑탄
                new MagazineInfoPreset{ id = 46711, name = "45mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 45mm 고폭탄
                new MagazineInfoPreset{ id = 46712, name = "76mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 76mm 철갑탄
                new MagazineInfoPreset{ id = 46713, name = "76mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 76mm 고폭탄
                new MagazineInfoPreset{ id = 46714, name = "85mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 85mm 철갑탄
                new MagazineInfoPreset{ id = 46715, name = "85mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 85mm 고폭탄
                new MagazineInfoPreset{ id = 46716, name = "100mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 100mm 철갑탄
                new MagazineInfoPreset{ id = 46717, name = "100mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 100mm 고폭탄
                new MagazineInfoPreset{ id = 46735, name = "122mm 철갑탄",iconUrl = "https://drive.google.com/uc?id=1EqetDkBr5BXXHoFVDfJ67rGGmThXZSRy"}, // 122mm 철갑탄
                new MagazineInfoPreset{ id = 46736, name = "122mm 고폭탄",iconUrl = "https://drive.google.com/uc?id=1pPNvhJ912wYYEVcKdcSCv3mO-YznU7X_"}, // 122mm 고폭탄
            }; // 탄창 정보 프리셋
            teamPresets = new TeamPresetTable[]
            {
                new TeamPresetTable
                {
                    teamName = "대한민국 육군",
                    teamImageUrl = "https://drive.google.com/uc?id=1DuskqHq8ySyRD03xsOnuDLPLiZiRrJUz",
                    teamIconUrl = "https://drive.google.com/uc?id=1NeiuYPwvBOjBWiMToZl6vlfachTgh-Bm",
                    classList = new ushort[] { 0,1,2,3,6,7 },
                    vehicleTypeList = new ushort[] { 0,6,1,2 },
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
                    classList = new ushort[] { 8,9,10,11,14,15 },
                    vehicleTypeList = new ushort[] { 3,7,4,5 },
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
                   explosiveList = new ushort[] { 2,0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 2,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 0. 한국군 소총병
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
                   primaryList = new ushort[] { 0, 2 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 2 },
                   equipmentInstanceID = 2,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 0,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46502, 46502, 46506, 46506, 46506, 46506, 46506 }
                }, // 1. 한국군 의생병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "돌격병",
                   iconUrl = "https://drive.google.com/uc?id=1oYDd031JV1Zv3GlTYKNAPUMixlzc06gh",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 2,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 4, 11 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 2,0,4,5 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = 4,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 2. 한국군 돌격병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "탄약수",
                   iconUrl = "https://drive.google.com/uc?id=13ZtLCd8APX3N1zIgLUl7j403Kaf67ZSA",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 9 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 2,0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 9,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 3. 한국군 탄약수
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "저격수",
                   iconUrl = "https://drive.google.com/uc?id=1XfuGHZvS83Qq--0KzSJGSVRTu9lDzTVO",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 4, 7 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 2,0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = 4,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 4. 한국군 저격수
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "공병",
                   iconUrl = "https://drive.google.com/uc?id=1ChuW7lkuehhKwwqRz_ocRZWR_XCGn3vy",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 4, 7 },
                   secondaryList = new ushort[] { 0 },
                   explosiveList = new ushort[] { 2,0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 4,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 5. 한국군 공병
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
                   explosiveList = new ushort[] { 2,0 },
                   equipmentInstanceID = 0,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 6,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = 0,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506, 277 }
                }, // 6. 한국군 운전병
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
                   explosiveList = new ushort[] { },
                   equipmentInstanceID = 4,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = ushort.MaxValue,
                   secondaryDefaultIndex = 0,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 0,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506, 277 }
                }, // 7. 한국군 전차병
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
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { 3,1 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 3,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = 1,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 8. 북한군 소총병
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
                   primaryList = new ushort[] { 1, 3 },
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { 3 },
                   equipmentInstanceID = 3,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 1,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46502, 46502,46506, 46506, 46506, 46506, 46506 }
                }, // 9. 북한군 의생병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "돌격병",
                   iconUrl = "https://drive.google.com/uc?id=1oYDd031JV1Zv3GlTYKNAPUMixlzc06gh",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 2,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 5, 8 },
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { 3,1,6,4 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 5,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = 1,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 10. 북한군 돌격병
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "탄약수",
                   iconUrl = "https://drive.google.com/uc?id=13ZtLCd8APX3N1zIgLUl7j403Kaf67ZSA",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 10 },
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { 3,1 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 10,
                   secondaryDefaultIndex = 1,
                   explosive_0DefaultIndex = 1,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 11. 북한군 탄약수
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "저격수",
                   iconUrl = "https://drive.google.com/uc?id=1XfuGHZvS83Qq--0KzSJGSVRTu9lDzTVO",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 5, 8 },
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { 3,1 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = 5,
                   secondaryDefaultIndex = 1,
                   explosive_0DefaultIndex = 1,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 12. 북한군 저격수
                new ClassPresetTable
                {
                   classType = EnumTable.EClassType.infantary,
                   name = "공병",
                   iconUrl = "https://drive.google.com/uc?id=1ChuW7lkuehhKwwqRz_ocRZWR_XCGn3vy",
                   timer = 0,
                   levelLimit = 0,
                   playerMax = 3,
                   supplyPoint = 10,
                   isMedic = true,
                   primaryList = new ushort[] { 5, 8 },
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { 3,1 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 5,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = 1,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506 }
                }, // 13. 북한군 공병
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
                   primaryList = new ushort[] { 12 },
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { 3,1 },
                   equipmentInstanceID = 1,
                   utilityList = new ushort[] { },
                   primaryDefaultIndex = 12,
                   secondaryDefaultIndex = ushort.MaxValue,
                   explosive_0DefaultIndex = 1,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 1,
                   utility_0DefaultIndex = ushort.MaxValue,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506, 277 }
                }, // 14. 북한군 운전병
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
                   primaryList = new ushort[] { 5 },
                   secondaryList = new ushort[] { 1 },
                   explosiveList = new ushort[] { },
                   equipmentInstanceID = 5,
                   utilityList = new ushort[] { 0 },
                   primaryDefaultIndex = ushort.MaxValue,
                   secondaryDefaultIndex = 1,
                   explosive_0DefaultIndex = ushort.MaxValue,
                   explosive_1DefaultIndex = ushort.MaxValue,
                   equipmentDefaultIndex = 0,
                   utility_0DefaultIndex = 0,
                   utility_1DefaultIndex = ushort.MaxValue,
                   loadoutList = new ushort[] { 46506, 46506, 277 }
                }, // 15. 북한군 전차병
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
                    tacticals = new ushort[] { 14 },
                    magazines = new ushort[] { 0 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 0,
                    gripDefaultIndex = ushort.MaxValue
                }, // 0.M1908스프링필드
                new PrimaryPresetTable
                {
                    name = "모신나강",
                    iconUrl = "https://drive.google.com/uc?id=1ZMwza9UbM7pqUAL2t-HR8-lTQLIBO-jd",
                    itemID = 46014,
                    supplyCost = 1,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] { 15 },
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
                    tacticals = new ushort[] { 13 },
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
                    tacticals = new ushort[] { 16 },
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
                    tacticals = new ushort[] { 12 },
                    magazines = new ushort[] { 6 },
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
                    tacticals = new ushort[] { 12 },
                    magazines = new ushort[] { 7 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 7,
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
                new PrimaryPresetTable
                {
                    name = "M1A1톰슨",
                    iconUrl = "https://drive.google.com/uc?id=1w4JTnNbMHZwQq2uMj_TasfPLARLVGKrK",
                    itemID = 46041,
                    supplyCost = 4,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 17, 18 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 17,
                    gripDefaultIndex = ushort.MaxValue
                }, // 11.M1A1톰슨
                new PrimaryPresetTable
                {
                    name = "SKS",
                    iconUrl = "https://drive.google.com/uc?id=1SkQugaeyLexBn2iZ9pxsnfayR6klAMRb",
                    itemID = 46044,
                    supplyCost = 2,
                    creditCost = 0,
                    amount = 1,
                    sights = new ushort[] {  },
                    tacticals = new ushort[] {  },
                    magazines = new ushort[] { 19 },
                    grips = new ushort[] {  },
                    sightDefaultIndex = ushort.MaxValue,
                    tacticalDefaultIndex = ushort.MaxValue,
                    magazineDefaultIndex = 19,
                    gripDefaultIndex = ushort.MaxValue
                }, // 12.SKS
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
                    name = "모신나강 클립",
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
                    supplyCost = 0,
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
                new LoadoutTable
                {
                    name = "총검",
                    iconUrl = "https://drive.google.com/uc?id=1zIYM0s69K2gJhYeFYCT5FioGqm2F6MSr",
                    itemID = 46034,
                    supplyCost = 1,
                    amount = 0,
                    amount_equipment = new byte[]{ }
                }, // 12. 카빈 총검
                new LoadoutTable
                {
                    name = "총검",
                    iconUrl = "https://drive.google.com/uc?id=1C1W6cEHWwq0Nmk2STOLIeqRIWql2X2RQ",
                    itemID = 46035,
                    supplyCost = 1,
                    amount = 0,
                    amount_equipment = new byte[]{ }
                }, // 13. 개런드 총검
                new LoadoutTable
                {
                    name = "총검",
                    iconUrl = "https://drive.google.com/uc?id=127Zc8VdJAWSu1WSMQ-GOZJw52KBiR6ol",
                    itemID = 46037,
                    supplyCost = 1,
                    amount = 0,
                    amount_equipment = new byte[]{ }
                }, // 14. 스프링필드 총검
                new LoadoutTable
                {
                    name = "총검",
                    iconUrl = "https://drive.google.com/uc?id=1QSDy5pYxL83Yc5t45w-v86S9o-_eMISd",
                    itemID = 46036,
                    supplyCost = 1,
                    amount = 0,
                    amount_equipment = new byte[]{ }
                }, // 15. 모신나강 총검
                new LoadoutTable
                {
                    name = "총검",
                    iconUrl = "https://drive.google.com/uc?id=11_ptgnfV-o_1mmUMF1zbPI6w0WEk92mH",
                    itemID = 46038,
                    supplyCost = 1,
                    amount = 0,
                    amount_equipment = new byte[]{ }
                }, // 16. SVT-40 총검
                new LoadoutTable
                {
                    name = "톰슨 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1v9pF0vyTR7-8VRORi8zEMufRxipknqAm",
                    itemID = 46042,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 4, 5, 6 }
                }, // 17. 톰슨 탄창
                new LoadoutTable
                {
                    name = "톰슨 드럼 탄창",
                    iconUrl = "https://drive.google.com/uc?id=1bKsQjLVCxGq7ef_HmLJYUlQy6xl2geGz",
                    itemID = 46043,
                    supplyCost = 2,
                    amount_equipment = new byte[]{ 3, 4, 5 }
                }, // 18. 톰슨 드럼 탄창
                new LoadoutTable
                {
                    name = "SKS 클립",
                    iconUrl = "https://drive.google.com/uc?id=1SUBPA0sIj88WD_FE4SlOJmPkYnbOW3tS",
                    itemID = 46045,
                    supplyCost = 0,
                    amount_equipment = new byte[]{ 5, 7, 9 }
                }, // 18. 톰슨 드럼 탄창
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
                new SecondaryPresetTable
                {
                    name = "TT-33",
                    iconUrl = "https://drive.google.com/uc?id=1Fflgrm1BLPmQKLcQ94UlSLbpVmp2M3Cq",
                    itemID = 46023,
                    supplyCost = 1,
                    amount = 1,
                    magazineItemID = 46024,
                    magazineAmount = 0, // 장구류 구분없이 탄창 똑같이 줄거면 이거 사용
                    amount_equipment = new byte[]{ 2, 3, 4 } // 장구류에 따른 탄창 개수
                }, // 1. TT-33
            }; // 부무장 프리셋
            explosivePresets = new SecondaryPresetTable[]
            {
                new SecondaryPresetTable
                {
                    name = "F1슈류탄",
                    iconUrl = "https://drive.google.com/uc?id=1mrElNeWellpyefWSXgoOSSlNThu7YcP8",
                    itemID = 46400,
                    amount = 1,
                    supplyCost = 1,
                    isDuplicatable = true
                }, // 0. F1 슈류탄
                new SecondaryPresetTable
                {
                    name = "RGD-33",
                    iconUrl = "https://drive.google.com/uc?id=1szoPYCYDDR6Abq4WsnjTDpHvPodPliBz",
                    itemID = 46401,
                    amount = 1,
                    supplyCost = 1,
                    isDuplicatable = true
                }, // 1. RGD-33
                new SecondaryPresetTable
                {
                    name = "M18연막탄",
                    iconUrl = "https://drive.google.com/uc?id=1EeBBZomyiygR05GdxppuskJonzTVwpuQ",
                    itemID = 46404,
                    amount = 1,
                    supplyCost = 1,
                    isDuplicatable = true
                }, // 2. M18연막탄(흰색)
                new SecondaryPresetTable
                {
                    name = "RGD-2연막탄",
                    iconUrl = "https://drive.google.com/uc?id=18b54H4VW6KD3vx0JMJnDZz6RhpM05GH2",
                    itemID = 46408,
                    amount = 1,
                    supplyCost = 1,
                    isDuplicatable = true
                }, // 3. RGD-2연막탄(흰색)
                new SecondaryPresetTable
                {
                    name = "M9A1바주카",
                    iconUrl = "https://drive.google.com/uc?id=1UfQq1OZZ52f-5sG1jCNlq-2hHLTtZJgY",
                    itemID = 46025,
                    amount = 1,
                    supplyCost = 5,
                    magazineItemID = 46026,
                    magazineAmount = 0,
                    amount_equipment = new byte[] { 0, 1, 2 },
                    isDuplicatable = false
                }, // 4. M9A1바주카
                new SecondaryPresetTable
                {
                    name = "M20바주카",
                    iconUrl = "https://drive.google.com/uc?id=1Lp5RJnABH7Fc1WVgQKqK4tU3XG219JhH",
                    itemID = 46027,
                    amount = 1,
                    supplyCost = 7,
                    magazineItemID = 46028,
                    magazineAmount = 0,
                    amount_equipment = new byte[] { 0, 1, 2 },
                    isDuplicatable = false
                }, // 5. M20바주카
                new SecondaryPresetTable
                {
                    name = "PTRS-41",
                    iconUrl = "https://drive.google.com/uc?id=1cFlpHhfeles_66ldxdSkH2Cqxt2x4fUi",
                    itemID = 46031,
                    amount = 1,
                    supplyCost = 3,
                    magazineItemID = 46032,
                    magazineAmount = 0,
                    amount_equipment = new byte[] { 2, 3, 4 },
                    isDuplicatable = false
                }, // 6. PTRS-41
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
                            equipment_Backpack = new ushort[]{ 46601 },
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
                            equipment_shirt = new ushort[]{ 46304 },
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
                            equipment_Vest = new ushort[]{ 46207 },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46304 },
                            equipment_Pant = new ushort[]{  },
                        },
                        new ClothPresetTable
                        {
                            name = "중무장",
                            iconUrl = "https://drive.google.com/uc?id=1lxXg35MPZQGxSuzhHEUMtD1gGCcLCAza",
                            supplyCost = 4,
                            equipment_Head = new ushort[]{ 46101 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46207 },
                            equipment_Backpack = new ushort[]{ 46600 },
                            equipment_shirt = new ushort[]{ 46304 },
                            equipment_Pant = new ushort[]{  },
                        }
                    }
                }, // 2. 한국군 의생병 장구류
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
                            equipment_shirt = new ushort[]{ 46328 },
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
                            equipment_Vest = new ushort[]{ 46217 },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46328 },
                            equipment_Pant = new ushort[]{  },
                        }, // 경무장
                        new ClothPresetTable
                        {
                            name = "중무장",
                            iconUrl = "https://drive.google.com/uc?id=1lxXg35MPZQGxSuzhHEUMtD1gGCcLCAza",
                            supplyCost = 4,
                            equipment_Head = new ushort[]{ 46106 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46217 },
                            equipment_Backpack = new ushort[]{ 46601 },
                            equipment_shirt = new ushort[]{ 46328 },
                            equipment_Pant = new ushort[]{  },
                        }, // 중무장
                    }
                }, // 3. 북한군 의생병 장구류
                new EquipmentPresetTable
                {
                    clothPresetList = new ClothPresetTable[]
                    {
                        new ClothPresetTable
                        {
                            name = "전차병 무장",
                            iconUrl = "https://drive.google.com/uc?id=1Q4i5y3qDo7ROTyWd-o4VxKqDsyv-Qr32",
                            supplyCost = 0,
                            equipment_Head = new ushort[]{ 46103 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46206 },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46305 },
                            equipment_Pant = new ushort[]{  },
                        }, // 전차병 무장
                    }
                }, // 4. 한국군 전차병 장구류
                new EquipmentPresetTable
                {
                    clothPresetList = new ClothPresetTable[]
                    {
                        new ClothPresetTable
                        {
                            name = "전차병 무장",
                            iconUrl = "https://drive.google.com/uc?id=1Q4i5y3qDo7ROTyWd-o4VxKqDsyv-Qr32",
                            supplyCost = 0,
                            equipment_Head = new ushort[]{ 46107 },
                            equipment_Mask = new ushort[] {},
                            equipment_Glasses = new ushort[]{  },
                            equipment_Vest = new ushort[]{ 46216 },
                            equipment_Backpack = new ushort[]{  },
                            equipment_shirt = new ushort[]{ 46327 },
                            equipment_Pant = new ushort[]{  },
                        }, // 전차병 무장
                    }
                }, // 5. 북한군 전차병 장구류
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
                    crewMinCount = 0,
                    classIndex = 0,
                    vehicleList = new ushort[]{ 6 },
                    respawnTime = 30,
                    abandonTime = 600,
                    destroyCost = 1,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = false
                }, // 0. 한국 수송차량
                new VehicleTypePresetTable
                {
                    name = "경전차",
                    iconUrl = "https://drive.google.com/uc?id=1v2FoM2sP5Zo4LPLrhLIpPxNvJlPjlOsy",
                    levelLimit = 2,
                    timer = 300,
                    vehicleMax = 2,
                    playerMinCount = 6,
                    crewMinCount = 2,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 0,12 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 3,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 1. 한국 경전차
                new VehicleTypePresetTable
                {
                    name = "중형전차",
                    iconUrl = "https://drive.google.com/uc?id=1_L86f8Z__AdHiVmwQ0UN6-Rb0XABO0qi",
                    levelLimit = 3,
                    timer = 300,
                    vehicleMax = 1,
                    playerMinCount = 6,
                    crewMinCount = 2,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 1,2 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 2. 한국 중형전차
                new VehicleTypePresetTable
                {
                    name = "수송차량",
                    iconUrl = "https://drive.google.com/uc?id=1lP1gm_r2Br-wb-OmO0HmMuY9wAtKarFs",
                    levelLimit = 0,
                    timer = 0,
                    vehicleMax = 5,
                    playerMinCount = 0,
                    crewMinCount = 0,
                    classIndex = 0,
                    vehicleList = new ushort[]{ 7 },
                    respawnTime = 30,
                    abandonTime = 600,
                    destroyCost = 1,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = false
                }, // 3. 북한 수송차량
                new VehicleTypePresetTable
                {
                    name = "경전차",
                    iconUrl = "https://drive.google.com/uc?id=1v2FoM2sP5Zo4LPLrhLIpPxNvJlPjlOsy",
                    levelLimit = 2,
                    timer = 300,
                    vehicleMax = 2,
                    playerMinCount = 6,
                    crewMinCount = 2,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 3,13 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 3,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 4. 북한 경전차
                new VehicleTypePresetTable
                {
                    name = "중형전차",
                    iconUrl = "https://drive.google.com/uc?id=1_L86f8Z__AdHiVmwQ0UN6-Rb0XABO0qi",
                    levelLimit = 3,
                    timer = 300,
                    vehicleMax = 1,
                    playerMinCount = 6,
                    crewMinCount = 2,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 4,5 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 5,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 5. 북한 중형전차
                new VehicleTypePresetTable
                {
                    name = "장갑차",
                    iconUrl = "https://drive.google.com/uc?id=1Hf2hXqMOS88r5tXkhUTYyLrB3GSpnjN8",
                    levelLimit = 0,
                    timer = 0,
                    vehicleMax = 2,
                    playerMinCount = 0,
                    crewMinCount = 2,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 8,9 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 2,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 6. 한국 장갑차
                new VehicleTypePresetTable
                {
                    name = "장갑차",
                    iconUrl = "https://drive.google.com/uc?id=1Hf2hXqMOS88r5tXkhUTYyLrB3GSpnjN8",
                    levelLimit = 0,
                    timer = 0,
                    vehicleMax = 2,
                    playerMinCount = 0,
                    crewMinCount = 2,
                    classIndex = 1,
                    vehicleList = new ushort[]{ 10,11 },
                    respawnTime = 30,
                    abandonTime = 240,
                    destroyCost = 2,
                    reward_score = 100,
                    reward_credit = 5,
                    classPlayerOnly = true
                }, // 7. 북한 장갑차
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
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                    ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46700 ,amount = 20 },
                        new VAmmoPresetTable{ itemID = 46701 ,amount = 20 },
                        new VAmmoPresetTable{ itemID = 46806 ,amount = 2 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                     supplyCooltime = 60
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
                    supplyCooltime = 60
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
                    supplyCooltime = 60
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
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
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
                new VehiclePresetTable
                {
                    name = "M20 정찰차량",
                    iconUrl = "https://drive.google.com/uc?id=1z4a1WTE5XbFmAZKrx6UpNL5zBaPIXet5",
                    itemID = 4614,
                    creditCost = 3,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                     ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46808 ,amount = 4 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                    supplyCooltime = 60
                }, // 8. M20 정찰장갑차
                new VehiclePresetTable
                {
                    name = "M8 그레이하운드",
                    iconUrl = "https://drive.google.com/uc?id=1CK2X1aDC7rZq5C5crcyIBL9dYme_r0zB",
                    itemID = 4612,
                    creditCost = 4,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포 사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "기관총 사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                     ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46700 ,amount = 15 },
                        new VAmmoPresetTable{ itemID = 46701 ,amount = 15 },
                        new VAmmoPresetTable{ itemID = 46808 ,amount = 2 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                    supplyCooltime = 60
                }, // 9. M20 정찰장갑차
                new VehiclePresetTable
                {
                    name = "BA-64",
                    iconUrl = "https://drive.google.com/uc?id=1noSh_c-y5FaNW_9qEMRTJDmfKZoAir7i",
                    itemID = 4615,
                    creditCost = 3,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                     ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46727 ,amount = 9 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                    supplyCooltime = 60
                }, // 10. BA-64
                new VehiclePresetTable
                {
                    name = "BA-11",
                    iconUrl = "https://drive.google.com/uc?id=1jB91tXkcgZfNZmwujbqFDK6nXG4dDqbS",
                    itemID = 4613,
                    creditCost = 4,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포 사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "기관총 사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                    },
                     ammos = new VAmmoPresetTable[]
                    {
                        new VAmmoPresetTable{ itemID = 46710 ,amount = 15 },
                        new VAmmoPresetTable{ itemID = 46711 ,amount = 15 },
                        new VAmmoPresetTable{ itemID = 46806 ,amount = 2 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                    supplyCooltime = 60
                }, // 11. BA-11
                new VehiclePresetTable
                {
                    name = "M24 채피",
                    iconUrl = "https://drive.google.com/uc?id=1xRrq-2JD8ic7WsdvWTpKoIHnajPLYcLe",
                    itemID = 4621,
                    creditCost = 7,
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
                        new VAmmoPresetTable{ itemID = 46727 ,amount = 2 },
                    },
                    isDeployable = false,
                    isSupplyable = true,
                     supplyCooltime = 60
                }, // 12. M24채피
                new VehiclePresetTable
                {
                    name = "T-70",
                    iconUrl = "https://drive.google.com/uc?id=1tUUpLPVN2U4DT4yJFS1OWDjKQRyPzBYp",
                    itemID = 4623,
                    creditCost = 7,
                    seats = new SeatPresetTable[]
                    {
                        new SeatPresetTable {name = "운전석",iconUrl = "https://drive.google.com/uc?id=1O6qQwzsByU_Ly2PxUf4O1MB2FGoiJVe8"},
                        new SeatPresetTable {name = "주포사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
                        new SeatPresetTable {name = "기관총사수",iconUrl = "https://drive.google.com/uc?id=1bVPCW1qAljYbqpUa29n5TgxGq2Ieskxy"},
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
                }, // 13. T-70
            };
            mapPresets = new MapPreset[]
            {
                new MapPreset
                {
                    name = "무명고지 전투",
                    mapImageUrl = "https://drive.google.com/uc?id=1WxdpEfcKw7XbEc_-DeoEcXKynnGU7ICH",
                    mapIconUrl = "https://drive.google.com/uc?id=1dhBERwnPd1yZlrty-pnxN5O0Smx5Dk6V",
                    mapSize = EnumTable.EMapSize.Large,
                    playerCount = 18,
                    // 0 = 자유, 1 = 섬멸전, 2 = 깃발점령전, 3 = 공방전
                    roundType = new byte[] { 2 ,3 },
                    mapPositon = new Vector2(960,-1984),
                    ObjectivePresets = new ObjectivePreset[]
                    {
                        new ObjectivePreset
                        {
                            position = new Vector3(1792,104,-1664), rotation = Quaternion.Euler(0,0,0), size = new Vector3(48,50,48),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1770,100,-1616), rotation = 152 }, // A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1794,101,-1676), rotation = 217 }, // A 거점 2번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1839,71,-1774), rotation = 334 }, // 1팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1865,70,-1766), rotation = 332 }, // 1팀 A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1884,68,-1743), rotation = 320 }, // 1팀 A 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1881,69,-1578), rotation = 234 }, // 2팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1869,64,-1564), rotation = 243 }, // 2팀 A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1851,65,-1549), rotation = 216 }, // 2팀 A 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(-0,0,-0), rotation = 0, size = new Vector3(0,0,0) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(1888,54,-1536), rotation = 0, size = new Vector3(64f,100f,64f) }
                        }, // A 거점
                        new ObjectivePreset
                        {
                            position = new Vector3(1664,81,-1536), rotation = Quaternion.Euler(0,0,0), size = new Vector3(48,50,48),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1647,78,-1542), rotation = 222 }, // B 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1648,77,-1556), rotation = 303 }, // B 거점 2번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1636,64,-1670), rotation = 348 }, // 1팀 B 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1652,67,-1663), rotation = 358 }, // 1팀 B 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1641,63,-1685), rotation = 354 }, // 1팀 B 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1714,54,-1416), rotation = 199 }, // 2팀 B 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1732,50,-1427), rotation = 213 }, // 2팀 B 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1700,61,-1412), rotation = 208 }, // 2팀 B 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(1632,62,-1696), rotation = 0, size = new Vector3(64f,100f,64f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(1727,50,-1408), rotation = 0, size = new Vector3(64f,100f,64f) }
                        }, // B 거점
                        new ObjectivePreset
                        {
                            position = new Vector3(1472,107,-1472), rotation = Quaternion.Euler(0,0,0), size = new Vector3(48,50,48),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1482,107,-1454), rotation = 173 }, // C 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1445,108,-1488), rotation = 360 }, // C 거점 2번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1473,74,-1589), rotation = 360 }, // 1팀 C 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1506,75,-1591), rotation = 360 }, // 1팀 C 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1445,73,-1595), rotation = 11 }, // 1팀 C 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1471,74,-1366), rotation = 180 }, // 2팀 C 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1442,74,-1366), rotation = 143 }, // 2팀 C 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1501,72,-1353), rotation = 208 }, // 2팀 C 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(1472,70,-1616), rotation = 0, size = new Vector3(64f,100f,64f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(1472,70,-1328), rotation = 0, size = new Vector3(64f,100f,64f) }
                        }, // C 거점
                        new ObjectivePreset
                        {
                            position = new Vector3(1280,81,-1408), rotation = Quaternion.Euler(0,0,0), size = new Vector3(48,50,48),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1297,78,-1407), rotation = 137 }, // D 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1281,79,-1400), rotation = 131 }, // D 거점 2번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1242,56,-1522), rotation = 24 }, // 1팀 D 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1226,54,-1522), rotation = 31 }, // 1팀 D 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1245,54,-1542), rotation = 11 }, // 1팀 D 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1305,68,-1260), rotation = 180 }, // 2팀 D 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1288,70,-1260), rotation = 155 }, // 2팀 D 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1319,69,-1274), rotation = 180 }, // 2팀 D 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(1216,50,-1536), rotation = 0, size = new Vector3(64f,100f,64f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(1312,67,-1248), rotation = 0, size = new Vector3(64f,100f,64f) }
                        }, // D 거점
                        new ObjectivePreset
                        {
                            position = new Vector3(1152,104,-1280), rotation = Quaternion.Euler(0,0,0), size = new Vector3(48,50,48),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1159.16f,97,-1274), rotation = 138 }, // E 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1183,96,-1319), rotation = 258 }, // E 거점 2번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1065,69,-1367), rotation = 60 }, // 1팀 E 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1087,72,-1388), rotation = 53 }, // 1팀 E 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1076,66,-1397), rotation = 47 }, // 1팀 E 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(1084,68,-1174), rotation = 138 }, // 2팀 E 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(1069,69,-1191), rotation = 145 }, // 2팀 E 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(1060,68,-1201), rotation = 132 }, // 2팀 E 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(1056,56,-1408), rotation = 0, size = new Vector3(64f,100f,64f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(0,0,-0), rotation = 0, size = new Vector3(0,0,0) }
                        }, // E 거점
                    },
                    basePos_0 = new Vector2(1792,-1896), // 0 팀 기지 마커 위치
                    basePos_1 = new Vector2(1152,-1088), // 1 팀 기지 마커 위치
                    baseSpawnPos_0 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(1789,59,-1840), rotation = 0 }, // 1 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(1807,59,-1842), rotation = 0 }, // 2 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(1827,57,-1844), rotation = 0 }, // 3 번 기지 스폰 위치

                    }, // 0 팀 기지 스폰 위치
                    baseSpawnPos_1 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(1178,55,-1107), rotation = 180 }, // 1 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(1159,53,-1103), rotation = 180 }, // 2 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(1134,51,-1100), rotation = 180 }, // 3 번 기지 스폰 위치

                    }, // 1 팀 기지 스폰 위치
                    baseRestrict_0 = new RestrictPreset{position = new Vector3(1728,30,-1856), rotation = 0, size = new Vector3(256f,100f,128f)},
                    baseRestrict_1 = new RestrictPreset{position = new Vector3(1216,30,-1088), rotation = 0, size = new Vector3(256f,100f,128f)},
                    vehicleSpawnPos_0 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(1763,35,-1895), rotation = 0 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(1777,35,-1901), rotation = 0 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(1787,35,-1903), rotation = 0 }, // 3 번 차량 스폰 위치
                    }, // 0 팀 차랑 스폰 위치
                    vehicleSpawnPos_1 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(1193,39,-1068), rotation = 180 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(1182,37,-1065), rotation = 180 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(1171,36,-1063), rotation = 180 }, // 3 번 차량 스폰 위치
                    }, // 1 팀 차랑 스폰 위치
                }, // 0. 무명고지 전투
                new MapPreset
                {
                    name = "교두보 전투",
                    mapImageUrl = "https://drive.google.com/uc?id=1Y1TxMKG9k3SqhBvXI0QaY8oq5Coe2BoE",
                    mapIconUrl = "https://drive.google.com/uc?id=1u1rfU4vkj6U5Tal9YX-NAHtMY3yoRfuZ",
                    mapSize = EnumTable.EMapSize.Medium,
                    playerCount = 12,
                    // 0 = 자유, 1 = 섬멸전, 2 = 깃발점령전, 3 = 공방전
                    roundType = new byte[] { 2 ,3 },
                    mapPositon = new Vector2(192,-1984),
                    ObjectivePresets = new ObjectivePreset[]
                    {
                        new ObjectivePreset
                        {
                            position = new Vector3(626,30,-1728), rotation = Quaternion.Euler(0,0,0), size = new Vector3(48,50,48),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(627,32,-1707), rotation = 145 }, // A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(587,32,-1711), rotation = 131 }, // A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(636,31,-1742), rotation = 180 }, // A 거점 3번 스폰
                                new SpawnPreset{ position = new Vector3(616,31,-1753), rotation = 260 }, // A 거점 4번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(633,47,-1820), rotation = 197 }, // 1팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(640,46,-1825), rotation = 221 }, // 1팀 A 거점 2번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(597,33,-1597), rotation = 80 }, // 2팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(633,33,-1604), rotation = 322 }, // 2팀 A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(634,34,-1567), rotation = 188 }, // 2팀 A 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(640,30,-1824), rotation = 0, size = new Vector3(64f,100f,32f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(624,30,-1600), rotation = 0, size = new Vector3(64f,100f,64f) }
                        }, // A 거점
                        new ObjectivePreset
                        {
                            position = new Vector3(448,49,-1728), rotation = Quaternion.Euler(0,350.5f,0), size = new Vector3(32,20,12),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(445,29,-1723), rotation = 180 }, // B 거점 1번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(512,32,-1844), rotation = 120 }, // 1팀 B 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(516,32,-1823), rotation = 131 }, // 1팀 B 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(548,31,-1822), rotation = 316 }, // 1팀 B 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(380,32,-1633), rotation = 269 }, // 2팀 B 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(358,31,-1640), rotation = 15 }, // 2팀 B 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(357,32,-1623), rotation = 124 }, // 2팀 B 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(512,30,-1824), rotation = 0, size = new Vector3(64f,100f,32f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(384,30,-1632), rotation = 0, size = new Vector3(64f,100f,32f) }
                        }, // B 거점
                        new ObjectivePreset
                        {
                            position = new Vector3(270,30,-1728), rotation = Quaternion.Euler(0,0,0), size = new Vector3(48,50,48),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(266,32,-1748), rotation = 282 }, // C 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(270,31,-1711), rotation = 305 }, // C 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(304,32,-1719), rotation = 271 }, // C 거점 3번 스폰
                                new SpawnPreset{ position = new Vector3(238,31,-1740), rotation = 256 }, // C 거점 3번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(292,31,-1839), rotation = 116 }, // 1팀 C 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(259,32,-1848), rotation = 177 }, // 1팀 C 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(279,31,-1864), rotation = 304 }, // 1팀 C 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(244,46,-1636), rotation = 0 }, // 2팀 C 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(222,46,-1617), rotation = 110 }, // 2팀 C 거점 2번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(272,30,-1856), rotation = 0, size = new Vector3(64f,100f,64f) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(256,30,-1632), rotation = 0, size = new Vector3(64f,100f,32f) }
                        }, // C 거점
                    },
                    basePos_0 = new Vector2(640,-1920), // 0 팀 기지 마커 위치
                    basePos_1 = new Vector2(256,-1536), // 1 팀 기지 마커 위치
                    baseSpawnPos_0 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(656,37,-1923), rotation = 0 }, // 1 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(623,38,-1917), rotation = 0 }, // 2 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(672,38,-1916), rotation = 350 }, // 3 번 기지 스폰 위치

                    }, // 0 팀 기지 스폰 위치
                    baseSpawnPos_1 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(232,39,-1526), rotation = 180 }, // 1 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(218,43,-1532), rotation = 170 }, // 2 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(266,41,-1536), rotation = 180 }, // 3 번 기지 스폰 위치

                    }, // 1 팀 기지 스폰 위치
                    baseRestrict_0 = new RestrictPreset{position = new Vector3(576,30,-1918), rotation = 0, size = new Vector3(128f,100f,64f)},
                    baseRestrict_1 = new RestrictPreset{position = new Vector3(320,30,-1536), rotation = 0, size = new Vector3(128f,100f,64f)},
                    vehicleSpawnPos_0 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(628,31,-1968), rotation = 0 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(616,32,-1964), rotation = 0 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(640,31,-1967), rotation = 0 }, // 3 번 차량 스폰 위치
                    }, // 0 팀 차랑 스폰 위치
                    vehicleSpawnPos_1 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(264,35,-1492), rotation = 180 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(278,37,-1496), rotation = 180 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(253,37,-1496), rotation = 180 }, // 3 번 차량 스폰 위치
                    }, // 1 팀 차랑 스폰 위치
                }, // 1. 교두보 전투
                new MapPreset
                {
                    name = "공장",
                    mapImageUrl = "https://drive.google.com/uc?id=17zrcDarmozhJ3lEpsaSmm4dTi9bxiEsW",
                    mapIconUrl = "https://drive.google.com/uc?id=153r3j02_Elgn9wMyZbWAFCpUsyGU0BN-",
                    mapSize = EnumTable.EMapSize.Small,
                    playerCount = 6,
                    // 0 = 자유, 1 = 섬멸전, 2 = 깃발점령전, 3 = 공방전
                    roundType = new byte[] { 2 },
                    mapPositon = new Vector2(-1088,-1984),
                    ObjectivePresets = new ObjectivePreset[]
                    {
                        new ObjectivePreset
                        {
                            position = new Vector3(-960,30,-1856), rotation = Quaternion.Euler(0,0,0), size = new Vector3(51,50,42),
                            objectiveSpawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(-952,31,-1855), rotation = 221 }, // A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(-973,31,-1864), rotation = 70 }, // A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(-968,31,-1843), rotation = 108 }, // A 거점 3번 스폰
                            },
                            team_0_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(-1038,35,-1917), rotation = 23 }, // 1팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(-1029,35,-1926), rotation = 40 }, // 1팀 A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(-1021,35,-1932), rotation = 35 }, // 1팀 A 거점 3번 스폰
                            },
                            team_1_spawn = new SpawnPreset[]
                            {
                                new SpawnPreset{ position = new Vector3(-873,38,-1795), rotation = 222 }, // 2팀 A 거점 1번 스폰
                                new SpawnPreset{ position = new Vector3(-884,37,-1790), rotation = 224 }, // 2팀 A 거점 2번 스폰
                                new SpawnPreset{ position = new Vector3(-894,37,-1779), rotation = 220 }, // 2팀 A 거점 3번 스폰
                            },
                            team_0_Restrict = new RestrictPreset{position = new Vector3(0,0,-0), rotation = 0, size = new Vector3(0,0,0) },
                            team_1_Restrict = new RestrictPreset{position = new Vector3(0,0,-0), rotation = 0, size = new Vector3(0,0,0) }
                        }, // A 거점
                    },
                    basePos_0 = new Vector2(-1088,-1984), // 0 팀 기지 마커 위치
                    basePos_1 = new Vector2(-832,-1728), // 1 팀 기지 마커 위치
                    baseSpawnPos_0 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(-1069,46,-1958), rotation = 45 }, // 1 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1075,47,-1952), rotation = 33 }, // 2 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1059,46,-1961), rotation = 41 }, // 3 번 기지 스폰 위치

                    }, // 0 팀 기지 스폰 위치
                    baseSpawnPos_1 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(-846,45,-1749), rotation = 225 }, // 1 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(-843,46,-1756), rotation = 223 }, // 2 번 기지 스폰 위치
                        new SpawnPreset{ position = new Vector3(-853,45,-1749), rotation = 223 }, // 3 번 기지 스폰 위치

                    }, // 1 팀 기지 스폰 위치
                    baseRestrict_0 = new RestrictPreset{position = new Vector3(-1088,30,-1984), rotation = 45, size = new Vector3(100f,100f,100f)},
                    baseRestrict_1 = new RestrictPreset{position = new Vector3(-832,30,-1728), rotation = 45, size = new Vector3(100f,100f,100f)},
                    vehicleSpawnPos_0 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(-1056,45,-1972), rotation = 90 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1066,44,-1972), rotation = 90 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-1076,44,-1973), rotation = 90 }, // 3 번 차량 스폰 위치
                    }, // 0 팀 차랑 스폰 위치
                    vehicleSpawnPos_1 = new SpawnPreset[]
                    {
                        new SpawnPreset{ position = new Vector3(-860,45,-1738), rotation = 270 }, // 1 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-850,45,-1738), rotation = 270 }, // 2 번 차량 스폰 위치
                        new SpawnPreset{ position = new Vector3(-840,44,-1738), rotation = 270 }, // 3 번 차량 스폰 위치
                    }, // 1 팀 차랑 스폰 위치
                }, // 3. 공장
            }; // 맵 프리셋
            gameModePresets = new GameModePreset[]
            {
                /*
                new GameModePreset
                {
                    name = "섬멸전",
                    playerCount = 24,
                    description = "적을 사살하여 상대의 점수를 낮춰 승리하는것이 목표",
                    iconUrl = "https://drive.google.com/uc?id=10MK6wcTRCGaDSlkNfQTjJ5SgxGEOJDj1",
                    maps = new byte[] { 0 },
                    scoreMultipier = 5
                }, // 0. 섬멸전
                */
                new GameModePreset
                {
                    name = "깃발점령전",
                    playerCount = 6,
                    description = "적을 사살하거나 거점을 점령하고 유지해 상대의 점수를 낯춰 승리하는것이 목표",
                    iconUrl = "https://drive.google.com/uc?id=14ZolrjQNQSJ8b1eVqOfPgle3sXoU0WK9",
                    maps = new byte[] { 2,1,0 },
                    scoreMultipier = 20
                }, // 0. 깃발점령전
                new GameModePreset
                {
                    name = "공방전",
                    playerCount = 12,
                    description = "공격 혹은 방어팀이 되어 각자 임무를 달성하는것이 목표",
                    iconUrl = "https://drive.google.com/uc?id=1dzB67DeAjJe_y04eI5Gav5xNnX8qG9M6",
                    maps = new byte[] { 1,0 },
                    scoreMultipier = 10
                }, // 1. 공방전
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
                4700
            }; // 보급 오브젝트
            supplyBarriacde = new ushort[]
            {
                47400
            };
            objectiveEffectGuid = new string[]
            {
                "dc3ddb05d89945088180c83bacaad4df",
                "e20d2fd427ea4c9da2945ee7b4e347d1",
                "30e33f96806b4f908501eecfd5b510d3",
                "8cc568f03f0d419ba9085fcc8686854b",
                "c436b8f3380d4bc4b4447cd12556a854"
            }; // 거점 이펙트 GUID
            discordUrl = "https://discord.gg/du53pnGG3z";
            teamPresetIndex_0 = 0;
            teamPresetIndex_1 = 1;
            respawnTimer = 5;
            classInterval = 24;
            teamRestrictCount = 2;
            spawnPos = new Vector3(1972.71f, 30.16f, 1974.63f);
            supplyCooltime_Inf = 60;
            spawnRot = 180f;
            teamChangeDelay = 0;
            freeModeReadyCount = 1;
            freeModeMapIndex = 2;
            // 거점 점령 관련 데이터
            baseCaptureSpeed = 3.34f;
            thresholdCaptureSpeed = 6.67f;
            maxCapturePlayerCount = 10;
        }
    }
}