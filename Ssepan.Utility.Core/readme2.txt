Ssepan.* Libraries v2.6.1


Purpose:
To encapsulate common functionality, reduce custom coding needed to start a new project, and provide consistency across projects. 


Usage notes:

~...


Enhancements:
2.7:
~Added SettingsComponent to Ssepan.Application. Use for settingsBase and any property in Settings that is an class that implements INotifyPropertyChanged.
~upgrade project to Framework 4.8
~change versioning to allow VS determinitic rules (no * wildcard)

2.6:
~All libraries are downgraded to .Net 3.5, except Ssepan.Web, which requires 4.0 for MVC feeatures.
~Updates Ssepan.Application to support new architecture, which is primarily MVC with a nod toward MVVM view-models. Support for WCF is on hold.

2.5:
~In Ssepan.Utilty, added some methods to ObjectHelper to reflect properties of an entity. These include GetExtensionMethods, GetTypeFromPropertyByPropertyName, GetValueFromPropertyByPropertyName, and GetPropertyNames.
~In Ssepan.Utilty, added a new class StringExtensions with a Contains method that allows the programmer to control case-sensitivity.
~In Ssepan.Data, added classes under Ssepan.Data.UI namespace to perform calculations related to paging, sorting, and searching an IEnumerable<T> for use in a UI.
~Added Ssepan.Web, with classes under the System.Web.Mvc.Html namespace to build the HTML controls and links for paging, sorting, and searching UI features.

2.4:
~In Ssepan.Io, added Folder class with DeleteFolderWithWait method.

2.3
~In Ssepan.Utility, added Cast method to ObjectHelper class to help with casting to anonymous types.

2.2:
~Modified Ssepan.Graphics to add static extension ImageExtensions to System.Drawing.Image; extensions provide access to ImageCodecInfo.
~Modified Ssepan.Transaction to add static GetDatedSubFolderNameFromDate method.

2.1:
~Ssepan.Messaging added to provide simple MSMQ functionality.
~Ssepan.Utility to fix log writing to include a fixed source of Ssepan.Utility and put the calling source into the description.
~Ssepan.Io with file io 'finally' fixes.
~Ssepan.Io to include additional constructor for FileDialogInfo to allow custom initial directory. Set CustomInitialDirectory to custom path and set InitialDirectory to Default(Environment.SpecialFolders).
~Ssepan.Io to include AutoUpgradeEnabled on open and save dialogs.
~Ssepan.Graphics to include image rotation.
~Sepan.Application to include _ValueChanging flag from sub-class, and to check and set it from the Controller base class Refresh method.
~Sepan.Application to support passing of delegate to ModelControllerBase.Save(); this is to allow additional custom processing after SettingsControllerBase.Save() and before Refresh().
~Sepan.Application to maintain new readonly property OldFilename, that gets set to Filename just before Filename is updated.
~Ssepan.Io.Graphics to exclude image scanning; scanning now resides in TwainLib.
~Ssepan.Collections to provide an extension method ShiftListItem.
~Cleaned up Tests folder and solution references, and deleted Tests_ folder.

2.0:
~Ssepan.Application refactored Settings / SettingsController bases, to put the static Settings property into the Settings class instead of the SettingsController class. This will make Settings more like SettingsController and the model / controller classes, and hopefully make Settings easier to understand and maintain.
~All libraries are upgraded to .Net 4.0.
~All libraries will use the same sub-version within the current version level, so that there is no confusion about which versions of individual libraries are tested together.

1.x
~Initial releases of libraries with other projects and as a complete set of source projects. Sub-versions varied by library project within the version 1.x level.

Fixes:

Known Issues:
~Running this app under Vista or Windows 7 requires that the library that writes to the event log (Ssepan.Utility.dll) have its name added to the list of allowed 'sources'. Rather than do it manually, the one way to be sure to get it right is to simply run the application the first time As Administrator, and the settings will be added. After that you may run it normally. To register additional DLLs for the event log, you can use this trick any time you get an error indicating that you cannot write to it. Or you can manually register DLLs by adding a key called '<filename>.dll' under HKLM\System\CurrentControlSet\services\eventlog\Application\, and adding the String value 'EventMessageFile' with the value like <C>:\<Windows>\Microsoft.NET\Framework\v2.0.50727\EventLogMessages.dll (where the drive letter and Windows folder match your system). Status: work-around. 

Possible Enhancements:
~TODO:Move more testing into MS Test framework. Status: Research.


Steve Sepan
ssepanus@yahoo.com
6/1/2014