using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Abstract;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using JM.LinqFaster;
using SharpDX;

namespace IconsBuilder
{
    public class MonsterIcon : BaseIcon
    {
        public MonsterIcon(Entity entity, GameController gameController, IconsBuilderSettings settings, Dictionary<string, Size2> modIcons)
            : base(entity, settings)
        {
            Update(entity, settings, modIcons);
        }

        public long ID { get; set; }

        public void Update(Entity entity, IconsBuilderSettings settings, Dictionary<string, Size2> modIcons)
        {
            Show = () => entity.IsAlive;
            if (entity.IsHidden && settings.HideBurriedMonsters)
            {
                Show = () => !entity.IsHidden && entity.IsAlive;
            }
            ID = entity.Id;

            if (!_HasIngameIcon) MainTexture = new HudTexture("Icons.png");

            switch (Rarity)
            {
                case MonsterRarity.White:
                    MainTexture.Size = settings.SizeEntityWhiteIcon;
                    break;
                case MonsterRarity.Magic:
                    MainTexture.Size = settings.SizeEntityMagicIcon;
                    break;
                case MonsterRarity.Rare:
                    MainTexture.Size = settings.SizeEntityRareIcon;
                    break;
                case MonsterRarity.Unique:
                    MainTexture.Size = settings.SizeEntityUniqueIcon;
                    break;
                default:
                    throw new ArgumentException(
                        $"{nameof(MonsterIcon)} wrong rarity for {entity.Path}. Dump: {entity.GetComponent<ObjectMagicProperties>().DumpObject()}");

                    break;
            }

            //if (_HasIngameIcon && entity.HasComponent<MinimapIcon>() && !entity.GetComponent<MinimapIcon>().Name.Equals("NPC") && entity.League != LeagueType.Heist)
            // return;

            if (!entity.IsHostile)
            {
                if (!_HasIngameIcon)
                {
                    MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterSmallGreenCircle);
                    Priority = IconPriority.Low;
                    Show = () => !settings.HideMinions && entity.IsAlive;
                }

                //Spirits icon
            }
            else if (Rarity == MonsterRarity.Unique && entity.Path.Contains("Metadata/Monsters/Spirit/"))
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeGreenHexagon);
            else
            {
                string modName = null;

                if (entity.HasComponent<ObjectMagicProperties>())
                {
                    var objectMagicProperties = entity?.GetComponent<ObjectMagicProperties>();
                    if (objectMagicProperties != null)
                    {

                        var mods = objectMagicProperties.Mods;

                        if (mods != null)
                        {
                            if (mods.Contains("MonsterConvertsOnDeath_")) Show = () => entity.IsAlive && entity.IsHostile;

                            modName = mods.FirstOrDefaultF(modIcons.ContainsKey);
                        }
                    }
                }

                if (modName != null)
                {
                    MainTexture = new HudTexture("sprites.png");
                    MainTexture.UV = SpriteHelper.GetUV(modIcons[modName], new Size2F(7, 8));
                    Priority = IconPriority.VeryHigh;
                }
                else
                {
                    switch (Rarity)
                    {
                        case MonsterRarity.White:
                            MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeRedCircle);
                            if (settings.ShowWhiteMonsterName)
                            {
                                Text = RenderName.Split(',').FirstOrDefault();
                                if (settings.ReplaceMonsterNameWithArchnemesis)
                                    GenerateArchNemString(entity, settings);
                            }

                            break;
                        case MonsterRarity.Magic:
                            MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeBlueCircle);
                            if (settings.ShowMagicMonsterName)
                            {
                                Text = RenderName.Split(',').FirstOrDefault();
                                if (settings.ReplaceMonsterNameWithArchnemesis)
                                    GenerateArchNemString(entity, settings);
                            }

                            break;
                        case MonsterRarity.Rare:
                            MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeYellowCircle);
                            if (settings.ShowRareMonsterName)
                            {
                                Text = RenderName.Split(',').FirstOrDefault();
                                if (settings.ReplaceMonsterNameWithArchnemesis)
                                    GenerateArchNemString(entity, settings);
                            }

                            break;
                        case MonsterRarity.Unique:
                            MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeCyanHexagon);
                            MainTexture.Color = Color.DarkOrange;
                            if (settings.ShowUniqueMonsterName)
                                Text = RenderName.Split(',').FirstOrDefault();

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"Rarity wrong was is {Rarity}. {entity.GetComponent<ObjectMagicProperties>().DumpObject()}");
                    }
                }
            }

            void GenerateArchNemString(Entity entity, IconsBuilderSettings settings)
            {
                #region archnemMods List<()>()
                var archnemMods = new List<(string, string)>()
                {
                    ("艾贝拉斯之触", "MonsterArchnemesisAbberath"),
                    ("艾尔卡莉之触", "MonsterArchnemesisArakaali_"),
                    ("奥术缓冲", "MonsterArchnemesisArcaneEnchantedMagic"),
                    ("奥术缓冲", "MonsterArchnemesisArcaneEnchanted"),
                    ("刺客", "MonsterArchnemesisAssassin"),
                    ("仁慈的守护者", "MonsterArchnemesisDivineTouched__"),
                    ("暴徒", "MonsterArchnemesisBerserker__"),
                    ("放血者", "MonsterArchnemesisBloodletterMagic"),
                    ("放血者", "MonsterArchnemesisBloodletter_"),
                    ("投弹手", "MonsterArchnemesisBombardier___"),
                    ("裂骨者", "MonsterArchnemesisBonebreakerMagic"),
                    ("裂骨者", "MonsterArchnemesisBonebreaker_"),
                    ("海洋王之触", "MonsterArchnemesisBrineKing___"),
                    ("奉献使徒", "MonsterArchnemesisConsecratorMagic"),
                    ("奉献使徒", "MonsterArchnemesisConsecration_"),
                    ("阴尸爆破", "MonsterArchnemesisCorpseExploder_"),
                    ("腐化者", "MonsterArchnemesisCorrupterMagic"),
                    ("腐化者", "MonsterArchnemesisCorrupter_"),
                    ("晶莹剔透", "MonsterArchnemesisLivingCrystals__"),
                    ("锐眼", "MonsterArchnemesisDeadeyeMagic"),
                    ("锐眼", "MonsterArchnemesisDeadeye"),
                    ("干旱先锋", "MonsterArchnemesisFlaskDrain__"),
                    ("雕像", "MonsterArchnemesisVoodooDoll"),
                    ("回声者", "MonsterArchnemesisEchoist___"),
                    ("电刑", "MonsterArchnemesisShockerMagic"),
                    ("电刑", "MonsterArchnemesisShocker_"),
                    ("强化元素", "MonsterArchnemesisCycleOfElements"),
                    ("增幅召唤物", "MonsterArchnemesisUnionOfSouls"),
                    ("尾随魔", "MonsterArchnemesisGraspingVines"),
                    ("刽子手", "MonsterArchnemesisExecutioner"),
                    ("最终叹息", "MonsterArchnemesisFinalGasp"),
                    ("烈焰编织", "MonsterArchnemesisFlameWalkerMagic"),
                    ("烈焰编织", "MonsterArchnemesisFlameWalker_"),
                    ("烈焰编织", "MonsterArchnemesisFlameTouched"),
                    ("丧心病狂", "MonsterArchnemesisRampage"),
                    ("永冻土", "MonsterArchnemesisFrostWalkerMagic"),
                    ("永冻土", "MonsterArchnemesisFrostWalker"),
                    ("冰霜编织", "MonsterArchnemesisFrostTouched_"),
                    ("巨像", "MonsterArchnemesisGargantuan"),
                    ("急速", "MonsterArchnemesisRaiderMagic"),
                    ("急速", "MonsterArchnemesisRaider_"),
                    ("先锋召唤物", "MonsterArchnemesisHeraldOfTheObelisk_"),
                    ("欧贝利斯克之捷", "MonsterArchnemesisHeraldOfTheObeliskMagic"),
                    ("咒术师", "MonsterArchnemesisHexer"),
                    ("混沌编织", "MonsterArchnemesisVoidTouched"),
                    ("冰牢", "MonsterArchnemesisGlacialCage"),
                    ("纵火", "MonsterArchnemesisIgniterMagic"),
                    ("纵火", "MonsterArchnemesisIgniter"),
                    ("善之触", "MonsterArchnemesisInnocence_____"),
                    ("勇士", "MonsterArchnemesisJuggernaut___"),
                    ("奇塔弗之触", "MonsterArchnemesisKitava"),
                    ("月神之触", "MonsterArchnemesisLunaris"),
                    ("熔岩屏障", "MonsterArchnemesisVolatileFlameBlood"),
                    ("憎恶", "MonsterArchnemesisOppressor_"),
                    ("魔灵吸取", "MonsterArchnemesisManaDonut"),
                    ("镜像幻影", "MonsterArchnemesisMirrorImage"),
                    ("死灵师", "MonsterArchnemesisNecromancer_"),
                    ("丰饶", "MonsterArchnemesisWealthy"),
                    ("超负荷", "MonsterArchnemesisChargeGenerator__"),
                    ("永冻土", "MonsterArchnemesisFreezerMagic"),
                    ("永冻土", "MonsterArchnemesisFreezer__"),
                    ("多彩", "MonsterArchnemesisPrismaticMagic"),
                    ("多彩", "MonsterArchnemesisPrismatic"),
                    ("振兴", "MonsterArchnemesisRejuvenating"),
                    ("哨兵", "MonsterArchnemesisDefenderMagic"),
                    ("哨兵", "MonsterArchnemesisDefender"),
                    ("夏卡莉之触", "MonsterArchnemesisShakari_"),
                    ("夏卡莉之触", "MonsterArchnemesisSolaris"),
                    ("魂灵牵引", "MonsterArchnemesisSoulConduit____"),
                    ("嗜魂者", "MonsterArchnemesisSoulEater_"),
                    ("灵魂行者", "MonsterArchnemesisSpiritWalkersMagic"),
                    ("分裂者", "MonsterArchnemesisMultiProjectiles"),
                    ("Splitting", "MonsterArchnemesisSplitting_"),
                    ("铸钢", "MonsterArchnemesisSteelAttuned___"),
                    ("暴风使徒", "MonsterArchnemesisLightningStorm"),
                    ("风行者", "MonsterArchnemesisLightningWalkerMagic"),
                    ("风行者", "MonsterArchnemesisLightningWalker_"),
                    ("风暴编织", "MonsterArchnemesisStormTouched_"),
                    ("短暂幻想", "MonsterArchnemesisTimeBubble"),
                    ("毒素", "MonsterArchnemesisPoisonerMagic"),
                    ("毒素", "MonsterArchnemesisPoisoner_"),
                    ("树人部落", "MonsterArchnemesisSaplings"),
                    ("诈欺师", "MonsterArchnemesisTrickster_"),
                    ("图克哈玛之触", "MonsterArchnemesisTukohama"),
                    ("聚魂", "MonsterArchnemesisUnionOfSoulsMagic"),
                    ("吸血魔", "MonsterArchnemesisVampiric"),
                };
                #endregion

                if (settings.ReplaceMonsterNameWithArchnemesis)
                {
                    var archNemText = "";
                    if (entity.HasComponent<ObjectMagicProperties>())
                    {
                        var objectMagicProperties = entity?.GetComponent<ObjectMagicProperties>();
                        if (objectMagicProperties != null)
                        {
                            var mods = objectMagicProperties.Mods;

                            if (mods != null)
                            {
                                foreach (var modText in mods)
                                {
                                    foreach (var value in archnemMods)
                                    {
                                        if (value.Item2.Contains(modText))
                                        {
                                            archNemText += "[" + value.Item1 + "] ";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(archNemText))
                        Text = archNemText;
                }
            }
        }
    }
}
