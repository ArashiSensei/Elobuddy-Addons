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
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("R", new CheckBox("Use R"));
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
            FarmMenu.Add("E", new CheckBox("Use E (Only if poisonned)"));
            FarmMenu.Add("eClear", new Slider("Cast E if Mana > ", 50));
            FarmMenu.AddGroupLabel("Spells in JungleClear");
            FarmMenu.Add("jungQ", new CheckBox("Use Q"));
            FarmMenu.Add("jungW", new CheckBox("Use W"));
            FarmMenu.Add("jungE", new CheckBox("Use E"));


            //Sub MiscMenu
            MiscMenu = CassioMenu.AddSubMenu("Other Settings");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("Gapcloser", new CheckBox("Use R on Gapcloser", false));
            MiscMenu.Add("Inter", new CheckBox("Use R to interrupt", false));


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
                
            }

            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Harass))
            {
             
            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
  

            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
 

            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {


            }



        }

        private static void Drawing_OnDraw(EventArgs args)
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
