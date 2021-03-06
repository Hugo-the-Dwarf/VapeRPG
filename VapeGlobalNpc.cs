﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;

using VapeRPG.Items;

namespace VapeRPG
{
    class VapeGlobalNpc : GlobalNPC
    {
        private const int expGainDistance = 3000; // The maximum distance at which players gain experience from mob kills

        /// <summary>
        /// Returns true if the mob is a chaos mob.
        /// </summary>
        public bool isChaos = false;

        /// <summary>
        /// Determines the stat scale of the NPC if it's a chaos mob.
        /// </summary>
        public int chaosMultiplier = 1;

        /// <summary>
        /// The override color for chaos mobs.
        /// </summary>
        public static Color ChaosColor = new Color(179, 104, 255, 127);

        private static Random rnd = new Random();

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            // Fix for incompatibility with other mods such as Calamity, etc.
            if (npc != null && Main.netMode == NetmodeID.Server || Main.netMode == NetmodeID.SinglePlayer)
            {
                if (!npc.boss && !npc.SpawnedFromStatue && !npc.friendly && !IsIgnoredTypeChaos(npc) && rnd.Next(0, 101) <= VapeConfig.ChaosChance)
                {
                    ChaosTransform(npc);
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (!IsIgnoredType(npc) && !npc.SpawnedFromStatue && !npc.friendly)
            {
                double gainedXp;
                if (npc.boss)
                {
                    foreach (Player player in Main.player.ToList().FindAll(x => x.active))
                    {
                        VapePlayer vp = player.GetModPlayer<VapePlayer>();
                        gainedXp = npc.lifeMax / 2;
                        vp.GainExperience((int)gainedXp);
                    }
                }
                else
                {
                    // If it isn't a boss, only nearby players gain experience based on the npc's HP
                    foreach (Player player in Main.player.ToList().FindAll(x => x.active))
                    {
                        if (Vector2.Distance(player.position, npc.position) <= expGainDistance)
                        {
                            VapePlayer vp = player.GetModPlayer<VapePlayer>();
                            gainedXp = 0.5 * Math.Pow(2, Math.Sqrt((2 * (1 + npc.defDamage / (2 * npc.lifeMax)) * npc.lifeMax) / Math.Pow(npc.lifeMax, 1 / 2.6)));
                            if (npc.lifeMax >= 1000)
                            {
                                gainedXp = npc.lifeMax / 2;
                            }
                            else if (npc.lifeMax <= 20)
                            {
                                gainedXp /= 2;
                            }
                            if (this.isChaos)
                            {
                                vp.GainExperience(1 + (int)(gainedXp / 3), true);
                                gainedXp *= (2 - 1 / this.chaosMultiplier);
                            }
                            vp.GainExperience((int)gainedXp);
                        }
                    }
                }
            }
            return base.CheckDead(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            #region UniqueDrops

            int chance;
            int spawnID;
            int itemID;

            if (npc.type == NPCID.EyeofCthulhu)
            {
                chance = rnd.Next(0, 3);

                if (chance == 0)
                {
                    spawnID = ItemID.GoldChainmail;
                }
                else if (chance == 1)
                {
                    spawnID = ItemID.GoldHelmet;
                }
                else
                {
                    spawnID = ItemID.GoldGreaves;
                }

                itemID = Item.NewItem(npc.position, spawnID);
                Main.item[itemID].GetGlobalItem<VapeGlobalItem>().Qualify(Main.item[itemID], ItemQuality.Unique);
            }

            if (npc.type == NPCID.SkeletronHead)
            {
                chance = rnd.Next(0, 9);

                if (chance == 0)
                {
                    spawnID = ItemID.AncientShadowScalemail;
                }
                else if (chance == 1)
                {
                    spawnID = ItemID.AncientShadowHelmet;
                }
                else if (chance == 2)
                {
                    spawnID = ItemID.AncientShadowGreaves;
                }
                else if (chance == 3)
                {
                    spawnID = ItemID.AncientCobaltBreastplate;
                }
                else if (chance == 4)
                {
                    spawnID = ItemID.AncientCobaltHelmet;
                }
                else if (chance == 5)
                {
                    spawnID = ItemID.AncientCobaltLeggings;
                }
                else if (chance == 6)
                {
                    spawnID = ItemID.NecroBreastplate;
                }
                else if (chance == 7)
                {
                    spawnID = ItemID.NecroHelmet;
                }
                else
                {
                    spawnID = ItemID.NecroGreaves;
                }

                itemID = Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, 0, 0), spawnID);
                Main.item[itemID].GetGlobalItem<VapeGlobalItem>().Qualify(Main.item[itemID], ItemQuality.Unique);
            }
            if (npc.type == NPCID.WallofFlesh)
            {
                chance = rnd.Next(0, 3);

                if (chance == 0)
                {
                    spawnID = ItemID.MoltenBreastplate;
                }
                else if (chance == 1)
                {
                    spawnID = ItemID.MoltenHelmet;
                }
                else
                {
                    spawnID = ItemID.MoltenGreaves;
                }

                itemID = Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, 0, 0), spawnID);
                Main.item[itemID].GetGlobalItem<VapeGlobalItem>().Qualify(Main.item[itemID], ItemQuality.Unique);
            }
            if (npc.type == NPCID.Retinazer || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime)
            {
                chance = rnd.Next(0, 5);

                if (chance == 0)
                {
                    spawnID = ItemID.HallowedPlateMail;
                }
                else if (chance == 1)
                {
                    spawnID = ItemID.HallowedHelmet;
                }
                else if (chance == 2)
                {
                    spawnID = ItemID.HallowedHeadgear;
                }
                else if (chance == 3)
                {
                    spawnID = ItemID.HallowedMask;
                }
                else
                {
                    spawnID = ItemID.HallowedGreaves;
                }

                itemID = Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, 0, 0), spawnID);
                Main.item[itemID].GetGlobalItem<VapeGlobalItem>().Qualify(Main.item[itemID], ItemQuality.Unique);
            }
            if (npc.type == NPCID.Golem)
            {
                chance = rnd.Next(0, 12);

                if (chance == 0)
                {
                    spawnID = ItemID.ShroomiteBreastplate;
                }
                else if (chance == 1)
                {
                    spawnID = ItemID.ShroomiteMask;
                }
                else if (chance == 2)
                {
                    spawnID = ItemID.ShroomiteHelmet;
                }
                else if (chance == 3)
                {
                    spawnID = ItemID.ShroomiteHeadgear;
                }
                else if (chance == 4)
                {
                    spawnID = ItemID.ShroomiteLeggings;
                }
                else if (chance == 5)
                {
                    spawnID = ItemID.SpectreRobe;
                }
                else if (chance == 6)
                {
                    spawnID = ItemID.SpectreHood;
                }
                else if (chance == 7)
                {
                    spawnID = ItemID.SpectreMask;
                }
                else if (chance == 8)
                {
                    spawnID = ItemID.SpectrePants;
                }
                else if (chance == 9)
                {
                    spawnID = ItemID.TurtleScaleMail;
                }
                else if (chance == 10)
                {
                    spawnID = ItemID.TurtleHelmet;
                }
                else
                {
                    spawnID = ItemID.TurtleLeggings;
                }

                itemID = Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, 0, 0), spawnID);
                Main.item[itemID].GetGlobalItem<VapeGlobalItem>().Qualify(Main.item[itemID], ItemQuality.Unique);
            }

            #endregion

            base.NPCLoot(npc);
        }

        public void ChaosTransform(NPC npc)
        {
            this.chaosMultiplier = rnd.Next(3, 6);

            npc.scale *= this.chaosMultiplier / 2.7f;
            npc.lifeMax *= this.chaosMultiplier;
            npc.life = npc.lifeMax;
            npc.defDamage *= this.chaosMultiplier;
            npc.defDefense *= this.chaosMultiplier / 2;
            npc.color = ChaosColor;
            npc.stepSpeed *= this.chaosMultiplier / 2f;

            this.isChaos = true;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, NetworkText.Empty, npc.whoAmI);

                ModPacket packet = this.mod.GetPacket();
                packet.Write((byte)VapeRPGMessageType.ServerTransformChaosNPC);
                packet.Write(this.chaosMultiplier);
                packet.Write(npc.whoAmI);
                packet.Send();
            }
        }

        public override void ResetEffects(NPC npc)
        {
            if (this.isChaos)
            {
                npc.GivenName = String.Format("Chaos {0}", npc.TypeName);
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if(type == NPCID.Merchant)
            {
                shop.item[nextSlot].SetDefaults(mod.ItemType<VapersGlobe>());
                nextSlot++;
            }
            base.SetupShop(type, shop, ref nextSlot);
        }

        private static bool IsIgnoredType(NPC npc)
        {
            return ignoredTypes.Contains(npc.type) ||
                npc.TypeName.ToLower().Contains("pillar");
        }

        private static bool IsIgnoredTypeChaos(NPC npc)
        {
            return ignoredTypes.Contains(npc.type) ||
                    ignoredTypesChaos.Contains(npc.type) ||
                    npc.TypeName.ToLower().Contains("head") ||
                    npc.TypeName.ToLower().Contains("body") ||
                    npc.TypeName.ToLower().Contains("tail") ||
                    npc.TypeName.ToLower().Contains("pillar") ||
                    npc.FullName.ToLower().Contains("head") ||
                    npc.FullName.ToLower().Contains("body") ||
                    npc.FullName.ToLower().Contains("tail") ||
                    npc.FullName.ToLower().Contains("pillar") ||
                    npc.GivenName.ToLower().Contains("head") ||
                    npc.GivenName.ToLower().Contains("body") ||
                    npc.GivenName.ToLower().Contains("tail") ||
                    npc.GivenName.ToLower().Contains("pillar") ||
                    npc.aiStyle == 6;
        }

        // Types to be ignored by experience gain/chaos transform
        private static int[] ignoredTypes =
        {
            NPCID.DungeonGuardian,
            NPCID.Bunny,
            NPCID.BunnySlimed,
            NPCID.BunnyXmas,
            NPCID.GoldBunny,
            NPCID.PartyBunny,
            NPCID.Penguin,
            NPCID.PenguinBlack,
            NPCID.Bird,
            NPCID.GoldBird,
            NPCID.ScorpionBlack,
            NPCID.Buggy,
            NPCID.Duck,
            NPCID.Duck2,
            NPCID.DuckWhite,
            NPCID.DuckWhite2,
            NPCID.Frog,
            NPCID.GoldFrog,
            NPCID.Worm,
            NPCID.GoldWorm,
            NPCID.TruffleWorm,
            NPCID.Goldfish,
            NPCID.GoldfishWalker,
            NPCID.Grasshopper,
            NPCID.GoldGrasshopper,
            NPCID.LightningBug,
            NPCID.Mouse,
            NPCID.GoldMouse,
            NPCID.Squirrel,
            NPCID.SquirrelGold,
            NPCID.SquirrelRed,
            NPCID.Scorpion,
            NPCID.Sluggy,
            NPCID.Snail,
            NPCID.GlowingSnail,
            NPCID.SeaSnail,
            NPCID.Butterfly,
            NPCID.GoldButterfly,
            NPCID.Firefly
        };

        private static int[] ignoredTypesChaos =
        {
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsHead,
            NPCID.EaterofWorldsTail,
            NPCID.DevourerBody,
            NPCID.DevourerHead,
            NPCID.DevourerTail,
            NPCID.GiantWormBody,
            NPCID.GiantWormHead,
            NPCID.GiantWormTail,
            NPCID.DuneSplicerBody,
            NPCID.DuneSplicerHead,
            NPCID.DuneSplicerTail,
            NPCID.LeechBody,
            NPCID.LeechHead,
            NPCID.LeechTail,
            NPCID.StardustWormBody,
            NPCID.StardustWormHead,
            NPCID.StardustWormTail,
            NPCID.SolarCrawltipedeBody,
            NPCID.SolarCrawltipedeHead,
            NPCID.SolarCrawltipedeTail,
            NPCID.SeekerBody,
            NPCID.SeekerHead,
            NPCID.SeekerTail,
            NPCID.DiggerBody,
            NPCID.DiggerHead,
            NPCID.DiggerTail,
            NPCID.TheDestroyerBody,
            NPCID.TheDestroyerTail,
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernHead,
            NPCID.WyvernLegs,
            NPCID.WyvernTail,
            NPCID.TombCrawlerBody,
            NPCID.TombCrawlerHead,
            NPCID.TombCrawlerTail,
            NPCID.BoneSerpentBody,
            NPCID.BoneSerpentHead,
            NPCID.BoneSerpentTail,
            NPCID.CultistDragonBody1,
            NPCID.CultistDragonBody2,
            NPCID.CultistDragonBody3,
            NPCID.CultistDragonBody4,
            NPCID.CultistDragonHead,
            NPCID.CultistDragonTail,
            NPCID.GolemHead,
            NPCID.GolemHeadFree,
            NPCID.MoonLordHead,
            NPCID.SkeletronHead
        };
    }
}
