using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private static AIHeroClient User = Player.Instance; // our player.

        // Declaring spells.

        // Q
        public static Spell.Skillshot Q;
        //W
        public static Spell.Skillshot W;
        //E
        public static Spell.Active E;
        //R      
        public static Spell.Targeted R;
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
            W = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 650, skillShotType: SkillShotType.Cone, castDelay: 25, spellSpeed: int.MaxValue, spellWidth: (int)(30 * 0.5));

            // E Spell Values
            E = new Spell.Active(spellSlot: SpellSlot.E);

            // R Spell Values
            R = new Spell.Targeted(spellSlot: SpellSlot.R, spellRange: 250);

            // Creating the MainMenu
            ChogathMenu = MainMenu.AddMenu("DatChogath", "DatChogath");

            ChogathMenu.AddGroupLabel("Hello and welcome to my Chogath Addon Version: " + AddonVersion + "!" );
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
            ComboMenu.Add("uoR", new CheckBox("Only Use R if Killable"));

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

            FarmMenu.AddGroupLabel("Spells in Farming Mode");
            FarmMenu.Add("Q", new CheckBox("Use Q"));
            FarmMenu.Add("qFarm", new Slider("Cast Q if Mana > ",50));
            FarmMenu.AddSeparator();
            FarmMenu.Add("W", new CheckBox("Use W"));
            FarmMenu.Add("wFarm", new Slider("Cast W if Mana > ", 50));
            FarmMenu.AddSeparator();
            FarmMenu.Add("R", new CheckBox("Stack R"));
            FarmMenu.Add("rFarm", new Slider("Stack R if Mana > ", 50));

            //Sub MiscMenu
            MiscMenu = ChogathMenu.AddSubMenu("Other Settings");

            MiscMenu.AddGroupLabel("Others features"); // TO DO ADD NEW FEATURES

            //Sub DrawMenu
            DrawMenu = ChogathMenu.AddSubMenu("Drawings Settings");

            DrawMenu.AddGroupLabel("Spells Range");
            DrawMenu.Add("Q", new CheckBox("Q Drawing"));
            DrawMenu.Add("W", new CheckBox("W Drawing"));
            DrawMenu.Add("R", new CheckBox("R Drawing"));
            DrawMenu.AddSeparator();
            DrawMenu.Add("allDr", new CheckBox("Disable all Drawings")); // FIND THE RIGHT WAY TO DISABLE ALL DRAWINGS



            // Triggers with core ticks
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
        }


            private static void Game_OnTick(EventArgs args)   // CODING ALL FEATURES
        {

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
                Drawing.DrawCircle(User.Position, R.Range, Color.Black);
            }
            else
            {
            }
        }







    }
    }


