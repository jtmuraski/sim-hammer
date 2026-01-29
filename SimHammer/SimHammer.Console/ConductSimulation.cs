//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices.Marshalling;
//using System.Text;
//using System.Threading.Tasks;
//using SimHammer.Core.Models.Simulation;
//using SimHammer.Core.Models.Units;
//using SimHammer.Core.Services.Simulation;
//using Spectre.Console;

//namespace SimHammer.Console
//{
//    public static class ConductSimulation
//    {
//        public static void BeginSimulation()
//        {
//            AnsiConsole.Clear();

//            AnsiConsole.MarkupLine("Enter the information for the attacking unit:");
//            Unit attacker = BuildUnit();

//            AnsiConsole.MarkupLine("Enter the information for the defending unit:");
//            Unit defender = BuildUnit();

//            int rounds = AnsiConsole.Prompt(
//                new TextPrompt<int>("How many rounds of combat would you like to simulate?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Rounds must be a positive integer![/]")
//                    .Validate(r => r > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Rounds must be greater than zero![/]")));

//            var combatType = new SelectionPrompt<string>()
//                .Title("Is this a melee or ranged combat simulation?")
//                .PageSize(10)
//                .AddChoices(new[] { "Melee", "Ranged" });

//            AnsiConsole.Clear();
//            AnsiConsole.MarkupLine("Please confirm the details of the simulation:");
//            DisplaySingleUnitTable(attacker, $"Attacker: {attacker.Name}");
//            DisplaySingleUnitTable(defender, $"Defender: {defender.Name}");

//            var confirmSettings = AnsiConsole.Console.Confirm("Ready to begin the simulation?");

//            if (confirmSettings)
//            {
//                SimulationService simulation = new SimulationService(simLogger);

//                SimulationResult result = new SimulationResult()
//                {
//                    Attacker = attacker,
//                    Defender = defender,
//                    TotalRounds = rounds,
//                    IsMeleeCombat = combatType.ToString() == "Melee"
//                };

//                AnsiConsole.Clear();

//                AnsiConsole.Progress()
//                    .Start(ctx =>
//                    {
//                        var task = ctx.AddTask("[green]Simulating Combat...[/]");
//                        task.MaxValue = rounds;
//                        for (int i = 1; i <= rounds; i++)
//                        {
//                            AnsiConsole.MarkupLine($"Round #{i}");
//                            CombatRound round = result.IsMeleeCombat ?
//                                simulation.SimulateMeleeCombatRound(attacker, defender, i) :
//                                simulation.SimulateRangedCombatRound(attacker, defender, i);
//                            result.CombatRounds.Add(round);

//                            AnsiConsole.MarkupLine($"Total Attacks #{round.TotalAttacksMade}");
//                            AnsiConsole.MarkupLine($"Total Hits: {round.TotalHits}");
//                            AnsiConsole.MarkupLine($"Total Wounds Inflicted: {round.TotalWoundsInflicted}");
//                            AnsiConsole.MarkupLine($"Total Damage Dealt: {round.TotalDamageDealt}");
//                            AnsiConsole.MarkupLine($"Total Saves Made: {round.TotalSavesMade}");
//                            AnsiConsole.MarkupLine($"Models Killed: {round.ModelsKilled}");
//                            task.Increment(1);
//                        }
//                    });

//                AnsiConsole.MarkupLine("Simulation Completed");
//                AnsiConsole.MarkupLine($"Total Rounds Simulated: {result.TotalRounds}");
//                AnsiConsole.MarkupLine($"Average Hits: {result.CombatRounds.Average(r => r.TotalHits)}");
//                AnsiConsole.MarkupLine($"Average Wounds: {result.CombatRounds.Average(r => r.TotalWoundsInflicted)}");
//                AnsiConsole.MarkupLine($"Average Damage: {result.CombatRounds.Average(r => r.TotalDamageDealt)}");

//                AnsiConsole.MarkupLine($"Averiage Models Killed: {result.CombatRounds.Average(r => r.ModelsKilled)}");

//                AnsiConsole.Confirm("Continue?");
//                return;

//            }
//            else
//            {
//                AnsiConsole.MarkupLine("[red]Simulation cancelled.[/]");
//                return;
//            }

//        }

//        public static Unit BuildUnit()
//        {
//            Unit unit = new Unit();

//            AnsiConsole.MarkupLine("Enter the Units values below.");

//            var name = AnsiConsole.Prompt(
//                new TextPrompt<string>("Unit Name?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Unit name cannot be empty![/]"));

//            var movement = AnsiConsole.Prompt(
//                new TextPrompt<int>("Movement (inches)?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Movement must be a positive integer![/]")
//                    .Validate(m => m > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Movement must be greater than zero![/]")));
//            unit.Name = name;

//            var strength = AnsiConsole.Prompt(
//                new TextPrompt<int>("Strength?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Strength must be a positive integer![/]")
//                    .Validate(s => s > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Strength must be greater than zero![/]")));
//            unit.Strength = strength;

//            var toughness = AnsiConsole.Prompt(
//                new TextPrompt<int>("Toughness?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Toughness must be a positive integer![/]")
//                    .Validate(t => t > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Toughness must be greater than zero![/]")));
//            unit.Toughness = toughness;

//            var wounds = AnsiConsole.Prompt(
//                new TextPrompt<int>("Wounds?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Wounds must be a positive integer![/]")
//                    .Validate(w => w > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Wounds must be greater than zero![/]")));
//            unit.Wounds = wounds;

//            var leadership = AnsiConsole.Prompt(
//                new TextPrompt<int>("Leadership?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Leadership must be a positive integer![/]")
//                    .Validate(l => l > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Leadership must be greater than zero![/]")));
//            unit.Leadership = leadership;

//            var save = AnsiConsole.Prompt(
//                new TextPrompt<int>("Armor Save?")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Armor Save must be a positive integer![/]")
//                    .Validate(s => s >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Armor Save cannot be negative![/]")));
//            unit.Save = save;

//            var invulnSave = AnsiConsole.Prompt(
//                new TextPrompt<int>("Invulnerable Save? If no Invuln save, enter 0")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Invulnerable Save must be a positive integer![/]")
//                    .AllowEmpty()
//                    .Validate(i => i >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Invulnerable Save cannot be negative![/]")));
//            unit.InvulnSave = invulnSave;

//            var modelCount = AnsiConsole.Prompt(
//                new TextPrompt<int>("Model Count? (Default is 1)")
//                    .PromptStyle("green")
//                    .ValidationErrorMessage("[red]Model Count must be a positive integer![/]")
//                    .Validate(mc => mc > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Model Count must be greater than zero![/]")));
//            unit.ModelCount = modelCount;

//            bool addRangedWeapons = AnsiConsole.Console.Confirm("Would you like to add ranged weapons to this unit?");
//            while (addRangedWeapons)
//            {
//                var weaponName = AnsiConsole.Prompt(
//                    new TextPrompt<string>("Ranged Weapon Name?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Weapon name cannot be empty![/]"));

//                var range = AnsiConsole.Prompt(
//                    new TextPrompt<int>("Range (inches)?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Range must be a positive integer![/]")
//                        .Validate(r => r > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Range must be greater than zero![/]")));

//                var ballisticSkill = AnsiConsole.Prompt(
//                    new TextPrompt<int>("Ballistic Skill?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Ballistic Skill must be a positive integer![/]")
//                        .Validate(bs => bs > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Ballistic Skill must be greater than zero![/]")));

//                var Attacks = AnsiConsole.Prompt(
//                    new TextPrompt<int>("Attacks?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Attacks must be a positive integer![/]")
//                        .Validate(a => a > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Attacks must be greater than zero![/]")));

//                var Strength = AnsiConsole.Prompt(
//                    new TextPrompt<int>("Strength?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Strength must be a positive integer![/]")
//                        .Validate(s => s > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Strength must be greater than zero![/]")));

//                var ArmourPiercing = AnsiConsole.Prompt(
//                    new TextPrompt<int>("Armour Piercing?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Armour Piercing must be a positive integer![/]")
//                        .Validate(ap => ap >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Armour Piercing cannot be negative![/]")));

//                var damage = AnsiConsole.Prompt(
//                    new TextPrompt<int>("Damage?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Damage must be a positive integer![/]")
//                        .Validate(d => d > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Damage must be greater than zero![/]")));

//                var quantity = AnsiConsole.Prompt(
//                    new TextPrompt<int>("Quantity?")
//                        .PromptStyle("green")
//                        .ValidationErrorMessage("[red]Quantity must be a positive integer![/]")
//                        .Validate(q => q > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Quantity must be greater than zero![/]")));

//                unit.RangedWeapons.Add(new RangedWeapon(weaponName, range, ballisticSkill, Attacks, Strength, ArmourPiercing, damage, quantity));

//                addRangedWeapons = AnsiConsole.Console.Confirm("Would you like to add ranged weapons to this unit?");
//            }

//            return unit;
//        }

//        // Display that stats for a single unit in a table format
//        public static void DisplaySingleUnitTable(Unit unit, string title)
//        {
//            var table = new Table()
//                .Border(TableBorder.Square)
//                .Title($"[green]{title}[/] Stats");

//            table.AddColumn(new TableColumn("[bold]Movement[/]").Centered());
//            table.AddColumn(new TableColumn("[bold]Strength[/]").Centered());
//            table.AddColumn(new TableColumn("[bold]Toughness[/]").Centered());
//            table.AddColumn(new TableColumn("[bold]Wounds[/]").Centered());
//            table.AddColumn(new TableColumn("[bold]Leadership[/]").Centered());
//            table.AddColumn(new TableColumn("[bold]Save[/]").Centered());
//            table.AddColumn(new TableColumn("[bold]Invulnerable Save[/]").Centered());
//            table.AddColumn(new TableColumn("[bold]Squad Size[/]").Centered());

//            table.AddRow(
//                unit.Movement.ToString(),
//                unit.Strength.ToString(),
//                unit.Toughness.ToString(),
//                unit.Wounds.ToString(),
//                unit.Leadership.ToString(),
//                unit.Save.ToString(),
//                unit.InvulnSave.ToString(),
//                unit.ModelCount.ToString());

//            AnsiConsole.Write(table);

//            // Display any ranged weapons that the unit has
//            if (unit.RangedWeapons.Count > 0)
//            {
//                var rangedTable = new Table()
//                    .Border(TableBorder.Square)
//                    .Title("[green]Ranged Weapons[/]");

//                rangedTable.AddColumn(new TableColumn("[bold]Weapon Name[/]").Centered());
//                rangedTable.AddColumn(new TableColumn("[bold]Range[/]").Centered());
//                rangedTable.AddColumn(new TableColumn("[bold]Ballistic Skill[/]").Centered());
//                rangedTable.AddColumn(new TableColumn("[bold]Attacks[/]").Centered());
//                rangedTable.AddColumn(new TableColumn("[bold]Strength[/]").Centered());
//                rangedTable.AddColumn(new TableColumn("[bold]AP[/]").Centered());
//                rangedTable.AddColumn(new TableColumn("[bold]Damage[/]").Centered());
//                rangedTable.AddColumn(new TableColumn("[bold]Quantity[/]").Centered());

//                foreach (var weapon in unit.RangedWeapons)
//                {
//                    rangedTable.AddRow(
//                        weapon.Name,
//                        weapon.Range.ToString(),
//                        weapon.BallisticSkill.ToString(),
//                        weapon.Attacks.ToString(),
//                        weapon.Strength.ToString(),
//                        weapon.ArmourPiercing.ToString(),
//                        weapon.Damage.ToString(),
//                        weapon.Quantity.ToString());
//                }

//                AnsiConsole.Write(rangedTable);
//            }

//        }
//    }
//}
