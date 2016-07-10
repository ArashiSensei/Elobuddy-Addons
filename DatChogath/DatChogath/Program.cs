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


namespace DatChogath
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;

        }
        public static Version AddonVersion;
        private static AIHeroClient User = Player.Instance; // player.

        // Declaring spells.

        // Q & W
        public static Spell.Skillshot Q,W;
        //E
        public static Spell.Active E;
        //R      
        public static Spell.Targeted R;
        //Ignite summoner
        public static SpellSlot Ignite { get; private set; }
        // Declaring Menus.

        private static Menu ChogathMenu, ComboMenu, HarassMenu, MiscMenu, FarmMenu, DrawMenu;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            AddonVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (User.ChampionName != "Chogath")
            {
                return;
            }

            // Q spell Values
            Q = new Spell.Skillshot(spellSlot: SpellSlot.Q, spellRange: 950, skillShotType: SkillShotType.Circular, castDelay: 650, spellSpeed: null, spellWidth: 250);

            // W Spell Values
            W = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 650, skillShotType: SkillShotType.Cone, castDelay: 25, spellSpeed: int.MaxValue, spellWidth: 250);

            // E Spell Values
            E = new Spell.Active(spellSlot: SpellSlot.E);

            // R Spell Values
            R = new Spell.Targeted(spellSlot: SpellSlot.R, spellRange: 175);

            //Ignite values
            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");

            // Creating the MainMenu
            ChogathMenu = MainMenu.AddMenu("DatChogath", "DatChogath");

            ChogathMenu.AddGroupLabel("Hello and welcome to my Chogath Addon Version: " + AddonVersion + "!");
            ChogathMenu.AddGroupLabel("Thanks to MeLoDaGg for his farming code, and Definitely not Kappa !");
            ChogathMenu.AddSeparator();
            ChogathMenu.AddGroupLabel("Arashi from Elobuddy forums !");

            // Sub ComboMenu
            ComboMenu = ChogathMenu.AddSubMenu("Combo Settings");

            ComboMenu.AddGroupLabel(" Spells in Combo Mode");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("R", new CheckBox("Use R"));
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Others Combo Settings");
            ComboMenu.Add("Ign", new CheckBox("Ignite if Killable ( WIP )"));

            // Sub HarassMenu
            HarassMenu = ChogathMenu.AddSubMenu("Harass Settings");

            HarassMenu.AddGroupLabel("Spells in Harass Mode");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("qHar", new Slider("Cast Q if Mana > ", 50));
            HarassMenu.AddSeparator();
            HarassMenu.Add("W", new CheckBox("Use W"));
            HarassMenu.Add("wHar", new Slider("Cast W if Mana > ", 50));

            //Sub FarmMenu
            FarmMenu = ChogathMenu.AddSubMenu("Farming Settings");

            FarmMenu.AddGroupLabel("Spells in LaneClear");
            FarmMenu.Add("Q", new CheckBox("Use Q"));
            FarmMenu.Add("qFarm", new Slider("Cast Q if Mana > ", 50));
            FarmMenu.Add("mqMin", new Slider("Cast Q if minions > ",1,3,6));
            FarmMenu.Add("W", new CheckBox("Use W"));
            FarmMenu.Add("wFarm", new Slider("Cast W if Mana > ", 50));
            FarmMenu.Add("mwMin", new Slider("Cast W if minions > ", 1, 3, 6));
            FarmMenu.AddGroupLabel("Spells in JungleClear");
            FarmMenu.Add("jQ", new CheckBox("Use Q"));
            FarmMenu.Add("jW", new CheckBox("Use W"));


            //Sub MiscMenu
            MiscMenu = ChogathMenu.AddSubMenu("Other Settings");

            MiscMenu.AddGroupLabel("Others features");
            MiscMenu.Add("blockR", new CheckBox("Block R if it wont kill ( WIP )"));
            MiscMenu.Add("StackR", new CheckBox("Auto Stack R ( WIP )"));
            MiscMenu.Add("FlashR", new CheckBox("Auto Flash R if killable ( WIP )"));
            MiscMenu.AddSeparator();
            MiscMenu.AddGroupLabel("Interruper Settings");
            MiscMenu.Add("Inter", new CheckBox("Interrupt all spells that it cans ( Q or W)"));


            //Sub DrawMenu
            DrawMenu = ChogathMenu.AddSubMenu("Drawings Settings");

            DrawMenu.AddGroupLabel("Spells Range");
            DrawMenu.Add("Q", new CheckBox("Q Drawing"));
            DrawMenu.Add("W", new CheckBox("W Drawing"));
            DrawMenu.Add("R", new CheckBox("R Drawing"));
            DrawMenu.AddSeparator();
            DrawMenu.AddGroupLabel("Global Drawings");
            DrawMenu.Add("allDr", new CheckBox("Disable all Drawings"));



            // Triggers with core ticks
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;

            Chat.Print("DatChogath " + AddonVersion + " by Arashi Loaded!");
        }


        private static void Game_OnTick(EventArgs args)   // CODING ultblock + flashr + stackR + Ignite
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
                IgniteUsage();

        }

        private static void Harass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (Target == null) return;
            {
                if (HarassMenu["Q"].Cast<CheckBox>().CurrentValue)
                {

                    var Qpred = Q.GetPrediction(Target);
                    if (!Target.IsValidTarget()) return;
                    if (Q.IsInRange(Target) && Q.IsReady() && Qpred.HitChance >= HitChance.High && ObjectManager.Player.ManaPercent >= HarassMenu["qHar"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(Target);
                    }
                }


                if (HarassMenu["W"].Cast<CheckBox>().CurrentValue)
                {

                    var Wpred = W.GetPrediction(Target);
                    if (!Target.IsValidTarget()) return;
                    if (W.IsInRange(Target) && W.IsReady() && Wpred.HitChance >= HitChance.High && ObjectManager.Player.ManaPercent >= HarassMenu["wHar"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(Target);
                    }

                }
            }
        }
        private static void Combo()
        {

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (Target == null) return;
            {
                if (ComboMenu["Q"].Cast<CheckBox>().CurrentValue)
                {

                    var Qpred = Q.GetPrediction(Target);
                    if (!Target.IsValidTarget()) return;
                    if (Q.IsInRange(Target) && Q.IsReady() && Qpred.HitChance >= HitChance.High)
                    {
                        Q.Cast(Target);
                    }
                }


                if (ComboMenu["W"].Cast<CheckBox>().CurrentValue)
                {

                    var Wpred = W.GetPrediction(Target);
                    if (!Target.IsValidTarget()) return;
                    if (W.IsInRange(Target) && W.IsReady() && Wpred.HitChance >= HitChance.High)
                    {
                        W.Cast(Target);
                    }

                }

                if (ComboMenu["R"].Cast<CheckBox>().CurrentValue)
                {

                    if (!Target.IsValidTarget()) return;
                    if (R.IsInRange(Target) && R.IsReady())
                    {
                        if (User.GetSpellDamage(Target, SpellSlot.R, 0) > Target.Health)
                        {
                            R.Cast(Target);
                        }

                    }

                }
            }

        }


        private static void IgniteUsage()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);


            if (!Q.IsOnCooldown && !W.IsOnCooldown && !R.IsOnCooldown) return;

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
            
            private static void JungleClear()                      // Credits to MeLoDaGg
        {
            var QJung = FarmMenu["jQ"].Cast<CheckBox>().CurrentValue;
            var WJung = FarmMenu["jW"].Cast<CheckBox>().CurrentValue;

            if (QJung)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(User.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (Q.IsReady() && QJung && minion != null)
                {
                    Q.Cast(minion.Position);
                }

                if (W.IsReady() && WJung && minion != null)
                {
                    W.Cast(minion.Position);
                }
            }

        }


        private static void LaneClear()                              // Credits to MeLoDaGg
        {
            var QLane = FarmMenu["Q"].Cast<CheckBox>().CurrentValue;
            var WLane = FarmMenu["W"].Cast<CheckBox>().CurrentValue;

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
                    if (enemyMinionsInRange >= FarmMenu["mqMin"].Cast<Slider>().CurrentValue && ObjectManager.Player.ManaPercent >= FarmMenu["qFarm"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(enemyMinion);
                    }
                }
                
                    if (W.IsReady() && WLane)
                    {


                    foreach (
                        var enemyMinion in
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(User) <= W.Range))
                    {
                        var enemyMinionsInRange =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                                .Count();
                        if (enemyMinionsInRange >= FarmMenu["mwMin"].Cast<Slider>().CurrentValue && ObjectManager.Player.ManaPercent >= FarmMenu["wFarm"].Cast<Slider>().CurrentValue)
                        {
                            W.Cast(enemyMinion.Position);
                        }
                    }

                }
            }
        }



        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var Inter = MiscMenu["Inter"].Cast<CheckBox>().CurrentValue;

            {
                if (Inter)
                {
                    if (sender.IsEnemy && W.IsReady() && sender.Distance(User) <= W.Range)
                    {
                        W.Cast(sender);

                    }
                    if (sender.IsEnemy && Q.IsReady() && sender.Distance(User) <= Q.Range)
                    {
                        Q.Cast(sender);
                    }


                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["AllDr"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Q.IsReady() && DrawMenu["Q"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(User.Position, Q.Range, Color.Purple);
            }
            else
            {
            }

            if (W.IsReady() && DrawMenu["W"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(User.Position, W.Range, Color.MediumPurple);
            }
            else
            {
            }

            if (R.IsReady() && DrawMenu["R"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(User.Position, R.Range, Color.MediumPurple);
            }
            else
            {
            }
        }
    }
}



