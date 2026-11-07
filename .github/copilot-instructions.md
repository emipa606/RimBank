# GitHub Copilot Instructions for RimBank Mod

## Mod Overview and Purpose

RimBank is a RimWorld mod that introduces banknotes as a new currency system to replace the traditional silver. The mod aims to provide a more efficient and convenient way to manage finances in the game. It adds banknotes as a new item, an ATM terminal building for currency exchange, and integrates with the game's trade system.

## Key Features and Systems

### Banknotes
- Denomination of 1,000 Silver, which remains constant.
- Lightweight and durable, suitable for saving large amounts of money.
- Accepted by all traders, with silver kept for change.
- Available in all purchases.
- New UI for deciding payment with banknotes.
- Orbital traders and settlements accept banknotes.
- Traders can give change in silver to avoid value overflows for banknotes.
- Exchangeable between silver and banknotes with any traders.

### RimBank ATM Terminal
- Minifiable and relatively inexpensive.
- Allows currency exchange between silver and banknotes at any time (with extra fees).
- Supports various extension softwares (requires sub mods).

### Extra Features
- Can be added or removed at any time without corrupting saves.
- Core mod and sub mods must have the same minor version number.

## Coding Patterns and Conventions

### C# Coding Standards
- Follow the official RimWorld coding standards and conventions.
- Use meaningful and descriptive variable and method names.
- Implement error handling where appropriate.
- Utilize Harmony patches for modding.

### XML Integration
- Use XML defs to define new items, buildings, and other game elements.
- Ensure XML defs are correctly formatted and integrated into the game.

### Harmony Patching
- Use Harmony to patch existing methods and add new functionalities.
- Follow best practices for patching to avoid conflicts with other mods.
- Use the `HarmonyPrefix`, `HarmonyPostfix`, and `HarmonyTranspiler` attributes as needed.

## Suggestions for Copilot

### C# File Examples

1. **CoreDefOf.cs**
   csharp
   namespace RimBank.Core
   {
       public static class CoreDefOf
       {
           public static ThingDef BankNote;
           public static ThingDef RimBankTerminal;
       }
   }
   

2. **ExtUtil.cs**
   csharp
   namespace RimBank.Trade.Ext
   {
       public static class ExtUtil
       {
           public static float BrokerageFactor { get; set; }
           public static void DoCountAdjustInterfaceForSilver()
           {
               // Implementation
           }
           public static void PrepareVirtualTrade()
           {
               // Implementation
           }
       }
   }
   

3. **Trader_BankNoteExchange.cs**
   csharp
   namespace RimBank.Trade.Ext
   {
       public class Trader_BankNoteExchange : Trader
       {
           public override void GiveSoldThingToPlayer(Thing soldThing)
           {
               // Implementation
           }
           public override void GiveSoldThingToTrader(Thing soldThing)
           {
               // Implementation
           }
       }
   }
   

4. **VirtualTrader.cs**
   csharp
   namespace RimBank.Trade.Ext
   {
       public class VirtualTrader : Trader
       {
           public override void GiveSoldThingToPlayer(Thing soldThing)
           {
               // Implementation
           }
           public override void GiveSoldThingToTrader(Thing soldThing)
           {
               // Implementation
           }
       }
   }
   

### XML File Examples

1. **StockGenerator_Banknotes.xml**
   xml
   <Defs>
       <thingDef>
           <defName>BankNote</defName>
           <label>Bank Note</label>
           <description>A note representing 1,000 silver.</description>
           <category>Item</category>
           <statBases>
               <MarketValue>10</MarketValue>
           </statBases>
       </thingDef>
   </Defs>
   

2. **Building_RimBankTerminal.xml**
   xml
   <Defs>
       <thingDef>
           <defName>RimBankTerminal</defName>
           <label>RimBank ATM Terminal</label>
           <description>A terminal for exchanging silver and banknotes.</description>
           <category>Building</category>
           <size>
               <x>1</x>
               <y>1</y>
           </size>
           <costList>
               <Steel>5</Steel>
               <Plasteel>3</Plasteel>
           </costList>
       </thingDef>
   </Defs>
   

By following these guidelines and examples, you can effectively use GitHub Copilot to assist with the development of the RimBank mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

