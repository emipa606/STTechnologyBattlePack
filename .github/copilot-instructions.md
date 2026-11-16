# GitHub Copilot Instructions for ST科技战斗包 - Technology Battle Pack (Continued)

## Mod Overview and Purpose

**Name:** ST科技战斗包 - Technology Battle Pack (Continued)  
**Author:** FD.FLY;LingLuo  
**Package ID:** Mlie.STTechnologyBattlePack  
**Mod Description:**  
This mod is an update of FD.FLY;LingLuos mod and introduces a collection of the latest black technologies in the ST zone. These technologies include various enhancements for combat, protection, and offensive capabilities. The mod aims to provide an immersive experience by integrating advanced weaponry and protective gear into the RimWorld game.

## Key Features and Systems

1. **Group Position**: A belt that, when activated, provides protection or abnormal status to surrounding friendly or enemy forces.
2. **Group Shield**: A belt that automatically opens to protect friendly forces around the character.
3. **Grenade Belt**: A belt that allows the use of grenades while using a gun, enhancing the accuracy of grenade usage.
4. **Vortex Shield**: A shield that passively reflects enemy bullets back, with smarter characters being able to deflect bullets more accurately.
5. **Energy Shield**: A shield that resists melee attacks while allowing the use of long-range weapons.
6. **Shooting Elves**: A belt that, when activated, copies the player's shots, effectively doubling the firepower.

## Coding Patterns and Conventions

- **Namespaces**: Use `Myth` as the primary namespace for all classes and methods.
- **Class Naming**: Follow PascalCase for class names, e.g., `Assist`, `Comp_StField`.
- **Method Naming**: Use camelCase for method names, e.g., `doShot`, `getTargetEquipmentFromProjectile`.
- **File Structure**: Place classes in appropriate folders, e.g., `Myth/Assist.cs`.
- **Comments**: Use XML comments for classes, methods, and properties to ensure clarity and maintainability.
- **Documentation**: Maintain a README file with detailed information about the mod's features, installation instructions, and troubleshooting tips.

## XML Integration

- **Defs**: Definitions for buildings, apparel, weapons, and hediffs are stored in XML files located in the `1.7/Defs` directory.
- **Structure**: Each XML file contains multiple `Def` elements, such as `ThingDef`, `HediffDef`, etc.
- **Attributes**: Ensure that all required attributes are present and correctly formatted in XML files.
- **Validation**: Use RimWorld's XML validation tools to ensure that your definitions are valid and compatible with the game engine.

## Harmony Patching

- **Harmony**: Utilize the Harmony library for patching existing game methods.
- **Patch Structure**: Create a separate class for each patch with methods marked as `[HarmonyPatch]`.
- **Patch Methods**: Ensure that patched methods are correctly implemented and do not interfere with the game's functionality.
- **Testing**: Thoroughly test patched methods to ensure they work as expected without causing crashes or unexpected behavior.

## Suggestions for Copilot

1. **Contextual Snippets**: Provide context-specific snippets for common tasks such as creating new `Def` elements, implementing Harmony patches, and adding new features.
2. **Code Templates**: Create templates for classes, methods, and XML files to streamline the development process.
3. **Documentation Integration**: Integrate documentation snippets for classes and methods to ensure that Copilot suggestions are well-documented.
4. **Error Handling**: Include error handling snippets for common issues such as null reference exceptions and invalid XML definitions.
5. **Community Contributions**: Encourage community contributions by suggesting ways to improve the mod and share best practices for mod development.

## Additional Resources

- **Discord Channel**: Use the Discord channel for error reporting and support.
- **Log Uploader**: Always post logs using the Log Uploader for better troubleshooting.
- **Mod Sorting**: Use RimSort to sort your mods for optimal compatibility.

By following these guidelines, developers can contribute effectively to the ST科技战斗包 - Technology Battle Pack (Continued) mod, ensuring that new features are integrated seamlessly and that existing functionalities are enhanced without introducing bugs.

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

