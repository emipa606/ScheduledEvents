# GitHub Copilot Instructions for Scheduled Events (Continued)

## Mod Overview and Purpose

The **Scheduled Events (Continued)** mod is an update to the original Fairs mod, enhancing the game by providing players with the ability to dynamically add and remove regularly scheduled events. This mod allows users to set up these events through the Options - Mod Settings - Scheduled Events menu, similar to creating incidents in scenarios but with additional options such as scheduling events to start after a specific time and adjusting settings during gameplay.

The mod is compatible with existing saves and has been tested with RimWorld version 1.0.2231. It is designed to provide players with more control over their gameplay, enabling them to create a more dynamic and engaging experience.

## Key Features and Systems

### Scheduled Events
- **Dynamic Addition and Removal**: Players can easily add and remove scheduled events.
- **Event Scheduling**: Events can be scheduled to start at specific times.
- **Run-time Adjustments**: Players can change event settings during gameplay.

### Integration with Existing Saves
The mod works seamlessly with existing RimWorld saves, ensuring that players can continue their existing games without issues.

## Coding Patterns and Conventions

### C# Code Structure
- **IncidentTarget.cs**: Manages incident targets and provides methods to get all incident definitions and the current target.
- **TickEvent.cs**: Handles the ticking of scheduled events and adds them to the event list.
- **Utils.cs**: Contains utility methods for drawing scale settings, logging debug information, and custom list serialization.
- **Startup.cs**: Initializes the mod.
- **TargetSelector.cs**: Manages target selection and execution.

### XML Integration
The mod does not currently use XML files for its definitions. However, it is designed to work with existing RimWorld XML definitions, ensuring compatibility with the game's data structures.

### Harmony Patching
Harmony patching is used to integrate the mod's functionality into the RimWorld game loop. Key patches include:
- **IncidentTarget**: Patches to handle incident targeting.
- **TickEvent**: Patches to manage scheduled event ticking.
- **TargetSelector**: Patches to handle target selection and execution.

## Suggestions for Copilot

### General Tips
1. **Code Completion**: Utilize Copilot for code completion when writing methods and classes related to event management, target selection, and utility functions.
2. **Error Handling**: Use Copilot to generate error handling code for logging and debugging purposes.
3. **Integration**: Use Copilot to assist in integrating new features with existing RimWorld systems through Harmony patches.

### Specific Features
- **Event Scheduling**: Use Copilot to generate code for scheduling events at specific times.
- **Dynamic Event Management**: Use Copilot to assist in implementing dynamic addition and removal of scheduled events.
- **Run-time Adjustments**: Use Copilot to generate code for adjusting event settings during gameplay.

### XML Definitions
Although the mod does not use XML files, use Copilot to assist in understanding and interacting with existing RimWorld XML definitions if needed for future enhancements.

## Conclusion

The **Scheduled Events (Continued)** mod provides players with enhanced control over their RimWorld games by allowing the dynamic addition and removal of regularly scheduled events. By following the coding patterns and conventions outlined above and utilizing GitHub Copilot, developers can efficiently extend and maintain the mod's functionality.

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

