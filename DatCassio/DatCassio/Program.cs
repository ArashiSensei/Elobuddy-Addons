using System;
using System.Reflection;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Color = System.Drawing.Color;
using Version = System.Version;

namespace DatCassio
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;

        }
        public static Version AddonVersion;
        private static AIHeroClient User = Player.Instance;
        public static Spell.Skillshot Q, W, R;
        public static Spell.Targeted E;
        public static SpellSlot Ignite { get; private set; }

        private static Menu CassioMenu, ComboMenu, HarassMenu, MiscMenu, FarmMenu, DrawMenu;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            AddonVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (User.ChampionName != "Cassiopeia")
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, 400, null, 75);

            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 250, null, 160);

            E = new Spell.Targeted(SpellSlot.E, 700);

            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, 600, null);

            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");

            CassioMenu = MainMenu.AddMenu("DatCassio", "DatCassio");

            CassioMenu.AddGroupLabel("Hello and welcome to my Cassio Addon Version: " + AddonVersion + "!");
            CassioMenu.AddGroupLabel("Thanks to trust in my addon !");
            CassioMenu.AddSeparator();
            CassioMenu.AddGroupLabel("Arashi from Elobuddy forums !");

            //ComboMenu
            ComboMenu = CassioMenu.AddSubMenu("Combo Settings");

            ComboMenu.AddGroupLabel(" Spells in Combo Mode");
            ComboMenu.Add("qCombo", new CheckBox("Use Q"));
            ComboMenu.Add("wCombo", new CheckBox("Use W"));
            ComboMenu.Add("eCombo", new CheckBox("Use E"));
            ComboMenu.Add("rCombo", new CheckBox("Use R"));
            ComboMenu.Add("rMin", new Slider("Cast R if x enemies", 2, 1, 5));
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Others Combo Settings");
            ComboMenu.Add("Ign", new CheckBox("Ignite if Killable"));


            //HarassMenu
            HarassMenu = CassioMenu.AddSubMenu("Harass Settings");

            HarassMenu.AddGroupLabel("Spells in Harass Mode");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("qHar", new Slider("Cast Q if Mana > ", 50));
            HarassMenu.AddSeparator();
            HarassMenu.Add("E", new CheckBox("Use E"));
            HarassMenu.Add("onlyE", new CheckBox(" Only Use E if poisonned"));
            HarassMenu.Add("eHar", new Slider("Cast E if Mana > ", 50));


            //FarmMenu
            FarmMenu = CassioMenu.AddSubMenu("Farming Settings");

            FarmMenu.AddGroupLabel("Last hit Settings");
            FarmMenu.Add("lasthitE", new CheckBox("Last it with E"));
            FarmMenu.AddGroupLabel("Spells in LaneClear");
            FarmMenu.Add("Q", new CheckBox("Use Q"));
            FarmMenu.Add("qClear", new Slider("Cast Q if Mana > ", 50));
            FarmMenu.Add("mqMin", new Slider("Cast Q if minions > ", 3, 1, 6));
            FarmMenu.Add("W", new CheckBox("Use W"));
            FarmMenu.Add("wClear", new Slider("Cast W if Mana > ", 50));
            FarmMenu.Add("mwMin", new Slider("Cast W if minions > ", 3, 1, 6));
            FarmMenu.Add("poisE", new CheckBox("Use E (Only if poisonned)"));
            FarmMenu.Add("eClear", new Slider("Cast E if Mana > ", 50));
            FarmMenu.AddGroupLabel("Spells in JungleClear");
            FarmMenu.Add("jungQ", new CheckBox("Use Q"));
            FarmMenu.Add("jungW", new CheckBox("Use W"));
            FarmMenu.Add("jungE", new CheckBox("Use E ( Only if poisonned)"));


            //Sub MiscMenu
            MiscMenu = CassioMenu.AddSubMenu("Other Settings");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("Gapcloser", new CheckBox("Use R on Gapcloser", false));
            MiscMenu.Add("Inter", new CheckBox("Use R to interrupt", false));
            MiscMenu.Add("eKS", new CheckBox("Make your ally mad with E (KS)"));
            MiscMenu.AddGroupLabel("Flee Settings");
            MiscMenu.Add("qFlee", new CheckBox("Use Q to move faster"));
            MiscMenu.Add("wFlee", new CheckBox("Use W to Slow enemies"));


            //Sub DrawMenu
            DrawMenu = CassioMenu.AddSubMenu("Drawings Settings");

            DrawMenu.AddGroupLabel("Spells Range");
            DrawMenu.Add("drawQ", new CheckBox("Q Drawing"));
            DrawMenu.Add("drawW", new CheckBox("W Drawing"));
            DrawMenu.Add("drawE", new CheckBox("E Drawing"));
            DrawMenu.Add("drawR", new CheckBox("R Drawing"));
            DrawMenu.AddSeparator();
            DrawMenu.AddGroupLabel("Global Drawings");
            DrawMenu.Add("allDr", new CheckBox("Disable all Drawings", false));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

            Chat.Print("<b><font color=\"#FF33D6\">DatCassio -</font></b> " + AddonVersion + " <b><font color=\"#FF33D6\">- By Arashi Loaded !</font></b>");
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();

            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();

            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
                
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();

            }
            eKS();
            IgniteUsage();



        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var useQ = ComboMenu["qCombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["wCombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["eCombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["rCombo"].Cast<CheckBox>().CurrentValue;

            if (target == null) return;
            {
                if (useR)
                {

                    var Rpred = R.GetPrediction(target);
                    if (!target.IsValidTarget()) return;
                    if (R.IsInRange(target) && Q.IsReady() && Rpred.HitChance >= HitChance.High && User.CountEnemiesInRange(R.Range) >= ComboMenu["rMin"].Cast<Slider>().CurrentValue)
                    {
                        R.Cast(target);
                    }
                }

                if (useQ)
                {

                    var Qpred = Q.GetPrediction(target);
                    if (!target.IsValidTarget()) return;
                    if (Q.IsInRange(target) && Q.IsReady() && Qpred.HitChance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }

                if (useW)
                {

                    var Wpred = W.GetPrediction(target);
                    if (!target.IsValidTarget()) return;
                    if (W.IsInRange(target) && Q.IsReady() && Wpred.HitChance >= HitChance.High)
                    {
                        W.Cast(target);
                    }

                }

                if (useE)
                {
                    if (!target.IsValidTarget()) return;
                    if (E.IsInRange(target) && E.IsReady() && PoisonnedTarget(target))
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var useQ = HarassMenu["Q"].Cast<CheckBox>().CurrentValue;
            var onlyE = HarassMenu["onlyE"].Cast<CheckBox>().CurrentValue;

            if (target == null) return;
            {
                if (useQ)
                {

                    var Qpred = Q.GetPrediction(target);
                    if (!target.IsValidTarget()) return;
                    if (Q.IsInRange(target) && Q.IsReady() && Qpred.HitChance >= HitChance.High && ObjectManager.Player.ManaPercent >= HarassMenu["qHar"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(target);
                    }
                }


                if (onlyE)
                {
                    if (!target.IsValidTarget()) return;
                    if (E.IsInRange(target) && E.IsReady() && ObjectManager.Player.ManaPercent >= HarassMenu["eHar"].Cast<Slider>().CurrentValue && PoisonnedTarget(target))
                    {
                        E.Cast(target);
                    }


                }
                else
                {
                    if (!target.IsValidTarget()) return;
                    if (E.IsInRange(target) && E.IsReady() && ObjectManager.Player.ManaPercent >= HarassMenu["eHar"].Cast<Slider>().CurrentValue)
                    {
                        E.Cast(target);
                    }

                }
            }

        }

        private static void LaneClear()
        {
            var QLane = FarmMenu["Q"].Cast<CheckBox>().CurrentValue;
            var WLane = FarmMenu["W"].Cast<CheckBox>().CurrentValue;
            var ELane = FarmMenu["poisE"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && QLane)
            {
                
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(User) <= Q.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinion.IsValidTarget() && enemyMinionsInRange >= FarmMenu["mqMin"].Cast<Slider>().CurrentValue && ObjectManager.Player.ManaPercent >= FarmMenu["qClear"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(enemyMinion);
                    }
                }
            }

                if (W.IsReady() && WLane)
                {
                    foreach (
                        var enemyMinion in
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(User) <= W.Range))
                    {
                        if (enemyMinion.IsValidTarget() && User.CountEnemyMinionsInRange(Q.Range) >= FarmMenu["mwMin"].Cast<Slider>().CurrentValue && ObjectManager.Player.ManaPercent >= FarmMenu["wClear"].Cast<Slider>().CurrentValue)
                        {
                            W.Cast(enemyMinion.Position);
                        }
                    }

                }

                if (E.IsReady() && ELane)
                {
                    foreach (
                        var enemyMinion in
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(User) <= E.Range))
                    {
                        if (enemyMinion.IsValidTarget() && ObjectManager.Player.ManaPercent >= FarmMenu["eClear"].Cast<Slider>().CurrentValue && PoisonnedMinion(enemyMinion))
                        {
                            E.Cast(enemyMinion);
                        }
                    }

                }
            }
        

    

        private static void LastHit()
        {

        }

        private static void JungleClear()
        {

            var QJung = FarmMenu["jungQ"].Cast<CheckBox>().CurrentValue;
            var EJung = FarmMenu["jungE"].Cast<CheckBox>().CurrentValue;
            var WJung = FarmMenu["jungW"].Cast<CheckBox>().CurrentValue;
            var minion =
                   EntityManager.MinionsAndMonsters.GetJungleMonsters(User.ServerPosition, 950f, true)
                       .FirstOrDefault();

            if (QJung)
            {
                
                if (Q.IsReady() && QJung && minion != null)
                {
                    Q.Cast(minion.Position);
                }
            }

            if (W.IsReady() && WJung && minion != null)
                {
                    W.Cast(minion.Position);
                }

            if (EJung)
            {

                if (E.IsReady() && EJung && minion != null && PoisonnedMinion(minion))
                {
                    Q.Cast(minion.Position);
                }
            }
        }



        

        private static void Flee()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var useQ = ComboMenu["qFlee"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["wFlee"].Cast<CheckBox>().CurrentValue;

            if (useQ)
            {

                var Qpred = Q.GetPrediction(target);
                if (!target.IsValidTarget()) return;
                if (Q.IsInRange(target) && Q.IsReady() && Qpred.HitChance >= HitChance.High)
                {
                    Q.Cast(target);
                }
            }

            if (useW)
            {

                var Wpred = W.GetPrediction(target);
                if (!target.IsValidTarget()) return;
                if (W.IsInRange(target) && Q.IsReady() && Wpred.HitChance >= HitChance.High)
                {
                    W.Cast(target);
                }

            }

        }

        private static void eKS()
        {

        }

        private static void IgniteUsage()
        {

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);


            if (Q.IsReady() && W.IsReady() && R.IsReady() && E.IsReady()) return;

            else
            {
                var useIgnite = ComboMenu["Ign"].Cast<CheckBox>().CurrentValue;

                if (useIgnite && target != null)
                {
                    if (User.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite) > target.Health)
                        User.Spellbook.CastSpell(Ignite, target);
                }
            }
        }

    

        public static bool PoisonnedTarget(Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Poison);
        }

        public static bool PoisonnedMinion(Obj_AI_Minion minion)
        {
            return minion.HasBuffOfType(BuffType.Poison);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!User.IsDead)
            {
                if (DrawMenu["AllDr"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (Q.IsReady() && DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(User.Position, Q.Range, Color.Purple);
                }
                else
                {
                }

                if (W.IsReady() && DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(User.Position, W.Range, Color.MediumPurple);
                }
                else
                {
                }

                if (E.IsReady() && DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(User.Position, E.Range, Color.MediumPurple);
                }
                else
                {
                }

                if (R.IsReady() && DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(User.Position, R.Range, Color.MediumPurple);
                }
                else
                {
                }
            }


        }
    }
}
