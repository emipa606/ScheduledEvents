# GitHub Copilot Instructions for RimWorld Mod: Scheduled Events (Continued)

## Mod Overview and Purpose
Scheduled Events (Continued) is an update of the Fairs mod designed for RimWorld, allowing players to dynamically add and remove regularly scheduled in-game events. The mod enhances gameplay by providing additional customization options akin to creating a scenario and choosing incidents, but with the flexibility to start events after a specific time and adjust settings during gameplay. The mod is compatible with existing saved games.

Current Version: 1.1.0  
Targeted RimWorld Version: 1.0.2231

## Key Features and Systems
- **Scheduled Events Setup**: Players can configure scheduled events through the "Options - Mod Settings - Scheduled Events" menu.
- **Dynamic Event Management**: Events can be adjusted on-the-fly, allowing for a tailored gameplay experience.
- **Compatibility**: The mod is designed to work with existing saves, providing seamless integration into ongoing games.
- **Event Customization**: Offers a range of options to tweak events, enhancing the depth and replayability of the game.

## Coding Patterns and Conventions
- **Class Naming**: Classes use PascalCase, and names are descriptive of their function (e.g., `ScheduledEvent`, `IncidentTarget`).
- **Method Naming**: Methods also use PascalCase, typically starting with a verb to indicate an action (e.g., `GetNextEventTick`, `ReloadEvents`).
- **Namespaces**: Use appropriate namespaces where applicable to organize code and prevent naming conflicts.
- **Coding Style**: Adopts standard C# coding styles, with clear method definitions and logical code separation.

## XML Integration
- **Mod Settings**: Settings for scheduled events are defined and configured in XML, allowing smooth integration with the RimWorld mod settings menu.
- **Data Registration**: XML files register incidents and configuration data, influencing how events are created and triggered.
- **Localization**: Ensure XML files include support for localization keys where necessary to support multilingual play.

## Harmony Patching
- **Patching**: Utilize Harmony for altering base game behavior without directly modifying game files, maintaining compatibility and minimizing conflicts with other mods.
- **Targeted Patches**: When using Harmony patches, focus on specific methods or operations to ensure limited and precise changes.

## Suggestions for Copilot
- **Code Completion**: Use GitHub Copilot to suggest method signatures, particularly for repeated patterns like accessing or manipulating game settings.
- **Event Logic**: Leverage the tool for drafting logic in calculating event timing or conditions.
- **XML Templates**: Let Copilot assist with generating templates for common XML structures, especially when registering multiple events or settings.
- **Harmony Assistance**: Suggest appropriate usage of Harmony annotations and patterns based on existing methods requiring patches.

This package aims to streamline mod development for RimWorld, enhancing user engagement by introducing diverse, customizable, and timely events within the game. As always, provide feedback through GitHub issues or comments to help improve compatibility and functionality. For any troubleshooting, ensure the mod and its requirements are isolated to identify conflicts, and use provided channels for reporting and support. Happy modding!
