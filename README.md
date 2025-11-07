# TestRail xUnit Integration Plugin

## 📋 Table of Contents

- [Overview](#-overview)
- [How It Works](#-how-it-works)
- [What This Plugin Offers](#-what-this-plugin-offers)
  - [For xUnit](#for-xunit)
  - [For Reqnroll](#for-reqnroll)
- [Installation & Setup](#%EF%B8%8F-installation--setup)
  - [xUnit Setup](#xunit-setup)
  - [Reqnroll Setup](#reqnroll-setup)
- [Tagging Tests](#%EF%B8%8F-tagging-tests)
  - [xUnit Tagging](#xunit-tagging)
  - [Reqnroll Tagging](#reqnroll-tagging)
- [Section Hierarchy Logic](#-section-hierarchy-logic)
- [General Notes](#-general-notes)
- [Display Name Formatting](#%EF%B8%8F-display-name-formatting)

---

## Overview

This plugin enables seamless integration between your xUnit-based test projects (including Reqnroll) and TestRail, allowing you to:

✅ Automatically publish test results to TestRail after test execution.

🏗️ Dynamically create missing TestRail structures (suites, sections, and test cases).

🔗 Map test methods to TestRail case IDs using simple custom attributes.

🔁 Continue using your existing `[Fact]` and `[Theory]` tests without requiring any changes.

---

## ❓ How It Works

The plugin collects metadata through custom attributes you add to your test methods. After all tests are executed, it:

1. Groups results by project and suite.
2. Creates any missing suites, sections, or test cases.
3. Publishes results in bulk to TestRail for efficiency and traceability.

This guide provides everything you need to get started—from annotating your tests to understanding how results are automatically pushed based on metadata.

---

## 🧩 What This Plugin Offers

### For xUnit

✅ No changes required for `[Fact]` / `[Theory]` tests

✅ Fully async, bulk result publishing.

✅ Automatic creation of Suites, Sections, and Cases.

✅ Metadata via attributes (`TestRailxUnitFramework`, `TestRailMetadata`, `TestRailInlineData`)

✅ Supports both class-level and method-level annotations.

✅ Optional InlineData support for case ID detection

✅ Supports Reqnroll also (see Reqnroll section below)

### For Reqnroll

✅ No need to include TestRail Case IDs manually — while Case IDs are still supported, it's recommended to let the plugin handle everything automatically.

✅ Automatic creation of Suites, Sections, and Cases directly in TestRail.

✅ Full feature parity with xUnit integration — no limitations or additional configuration required.

---

## 🛠️ Installation & Setup

### xUnit Setup

#### 1. Install the NuGet Package (Mandatory)
Add the project as as reference to your solution
or From NuGet.org, always install latest:

```powershell
Install-Package Zaghloul.QA.TestRail.xUnit
```

The plugin supports both **.NET 6** & **.NET 8**.

#### 2. Register the Custom Framework (Mandatory)

Add the attribute at the assembly level in your test project, in any existing class or a new one (e.g., `AssemblyInfo.cs`):

```csharp
[assembly: TestRailxUnitFramework]
```

#### 3. Add Configuration (Mandatory Step)

Add a `testrailSettings.json` file to your test project, and set **Copy to Output Directory** to **Always**:

```json
{
  "Email": "email@domain.com",
  "ApiKey": "123456-XXXXXXXXXXXXXXXXXX",
  "PublishResultsEnabled": true,
  "ProjectId": "1",
  "DefaultSectionName": "Test Cases",
  "MilestoneName": "PI 25.2",
  "CloseRun": true,
  "AddOnlyExecutedCasesToRun": true,
  "ServiceName": "ServiceNameOrAppName"
}
```

**Required Parameters:**
- `Email`: Your TestRail team account.
- `ApiKey`: Either your TestRail password or an API Key (both are supported).
- `ProjectId`: The TestRail Project ID.

**Optional Parameters:**
- `DefaultSectionName`: Default section name to use if no section is provided.
- `MilestoneName`: Milestone name, if you want a milestone to be created, used or applied.
- `CloseRun`: Whether to close the run after pushing results.
- `ServiceName`: Service name to be used in Run name if provided.
- `AddOnlyExecutedCasesToRun`: When true, adds only the executed tests to Run, rather than all Suite cases.

##### 3.1. How to Get an API Key?

1. Log in to TestRail, then go to **My Settings**.
2. Under **API Keys**, click **Add Key**, give it a name, click **Generate Key**, and then **Save Configuration**.

##### 3.2. Managing Shared Configuration Across Multiple Projects

If your solution contains multiple projects, it's inefficient to duplicate the same configuration file in each one. Instead, you can maintain a single shared config file and link it across all projects.

**Step 1: Place the Shared Configuration File**

Store the shared JSON file in a centralized location at the solution level, for example:

```
.test/testrailSettings.json
```

**Step 2: Link the File in Each Project's .csproj**

In each project that requires access to this config, add the following to the `.csproj` file:

```xml
<ItemGroup>
  <None Include="..\.test\testrailSettings.json">
    <Link>testrailSettings.json</Link>
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

This approach ensures consistency and maintainability by keeping the configuration centralized and automatically available to all relevant projects.

---

### Reqnroll Setup

#### 1. Install the NuGet Package (Mandatory)

Add the project as as reference to your solution or install latest available nuget:

```powershell
Install-Package Zaghloul.QA.TestRail.xUnit
```

The plugin supports both **.NET 6** & **.NET 8**.

#### 2. Add Configuration (Mandatory Step)

Add a `testrailSettings.json` file to your test project (same as xUnit setup above).

#### 3. Add Binding Assembly to reqnroll.json (Mandatory)

Add the below assembly binding to your `reqnroll.json` file:

```json
{
  "bindingAssemblies": [
    {
      "assembly": "Zaghloul.QA.TestRail.xUnit"
    }
  ]
}
```

---

## 🏷️ Tagging Tests

### xUnit Tagging

#### A. Class-Level Metadata

Add the `TestRailMetadata` attribute to the class.

**Only Suite name:**

```csharp
[TestRailMetadata("SuiteName")]
```

**Suite name and Section name:**

```csharp
[TestRailMetadata("SuiteName", "SectionName")]
```

**Example:**

```csharp
[TestRailMetadata("Login", "Auth")]
public class LoginTests
{
    [Fact]
    public void Should_Login_With_Valid_Credentials() { }
}
```

#### B. Method-Level Metadata

The same as class-level tagging, but applied to individual methods.

```csharp
public class ProfileTests
{
    [Fact]
    [TestRailMetadata("User", "Profile")]
    public void Should_Update_Profile() { }
}
```

#### C. Parameterized Tests

> **Note:** If you provided class level metadata, you don't need to provide tags on parameterized tests unless you want to override them or have Case IDs you want to provide. If you're fine with the plugin creating everything, you can use option "A" (class-level) and it will apply to all methods, ignoring any method-level attributes.

If you are using `[Theory]`, you must provide the `TestRailMetadata` attribute.

You have two options depending on your needs:

##### C.1. Theory and AutoData

```csharp
public class PaymentTests
{
    [Theory]
    [AutoData]
    [TestRailMetadata("Purchase", "Payment")]
    public void Should_Process_Payment(int amount, string currency) { }
}
```

##### C.2. Theory and InlineData

```csharp
public class NotificationTests
{
    [Theory]
    [InlineData("user@example.com", "Welcome")]
    [InlineData("user@example.com", "Reminder")]
    [TestRailMetadata("Notifications", "Emails")]
    public void Should_Send_Email(string to, string subject) { }
}
```

#### D. Adding Case IDs

By default, if no case ID is provided, the plugin will search for (or create) the appropriate Suite, Section, and Case.

If you already have TestRail cases and want to link tests directly, you can specify Case IDs.

##### D.1. Fact Tests with Case ID

Provide suite name and Case ID(s) (must start with "C").

```csharp
public class ProfileTests
{
    [Fact]
    [TestRailMetadata("User", ["C302358"])]
    public void Should_Update_Profile() { }
}
```

##### D.2. Theory Tests with Case ID

For Theory you have two options:

1. Use the xUnit `InlineData` and pass case ID as last parameter
2. Use `TestRailInlineData` from the plugin (recommended)

The difference is that with `TestRailInlineData`, you pass case ID(s) as an array as the first parameter, then pass your normal test parameters normally. With this approach, you don't need to add an extra argument/parameter to the method itself.

**Examples:**

```csharp
public class NotificationTests
{
    // Using normal InlineData (extra method argument required)
    [Theory]
    [InlineData("user@example.com", "Welcome", "C123456")]
    [InlineData("user@example.com", "Reminder", "C56789")]
    [TestRailMetadata("Notifications", "Emails")]
    public void Should_Send_Email(string to, string subject, string caseId) { }

    // Using TestRailInlineData (no extra method argument)
    [Theory]
    [TestRailInlineData(["C302359"], "user@example.com", "Welcome")]
    [TestRailInlineData(["C302360", "C302361"], "user@example.com", "Reminder")]
    [TestRailMetadata("Notifications", "Emails")]
    public void Should_Send_Email(string to, string subject) { }
}
```

---

### Reqnroll Tagging

Tagging your tests is simple and effortless.

Just annotate your feature files with **Suite** and **Section** tags (you can use either `Suite`/`Section` or `SuiteName`/`SectionName`) on the **feature**, not scenarios.

**Example:**

```gherkin
@Suite:Example-Api-FunctionalTests
@Section:ExampleTests
Feature: TestFeature
    As a registered customer
    I want to get the list of products

Scenario Outline: Get a list of available products 
    Given a Customer with id "1"
```

---

## 🌲 Section Hierarchy Logic

You can create nested sections by providing slashed names in the section parameter:

```csharp
[Theory]
[InlineData(1)]
[TestRailMetadata("TestSuiteA", "ParentSection/SubSectionA/SubSectionB")]
public void Add_NullInput_ThrowsArgumentNullException(int x)
{
    int? a = null, b = null;
    Assert.Throws<ArgumentNullException>(() => _mathHelper.Add(a ?? 0, b ?? 0));
}
```

`ParentSection/SubSectionA/SubSectionB` will automatically create the full hierarchy if any level does not already exist (you can also do the same for Reqnroll).

---

## 📝 General Notes

### xUnit Notes

- Metadata can be defined at class, method, or parameter level.
- Tests are grouped by Suite/Section.
- Runs are created per suite and closed if `CloseRun` is true.
- Suites, Sections, Test Cases, and Milestones are created on-the-fly if missing.
- Class-level attribute overrides method-level attributes.
- Case IDs are optional unless you need explicit mapping.
- If no Section is provided, the plugin will use the `DefaultSectionName` from configuration. If none exists, case creation will be skipped.
- Milestones are created if not found and attached to runs.
- Tests without plugin attributes are ignored.
- All results are sent in bulk after all tests complete.

### Reqnroll Notes

- **Reqnroll v3.1.2 or higher** is required.
- Ensure you've added the binding assembly as shown in the setup section.
- Features without a `Suite` tag will be ignored and not published to TestRail.

---

## ✏️ Display Name Formatting

To improve readability, underscores are replaced with spaces when creating TestRail cases.

**Example:**

```
Should_Process_Payment (100, "USD") 
  ➔ Should Process Payment (p1: 100, p2: USD)
```
---

**Plugin Version:** 2.0.0+  
**Supported Frameworks:** xUnit, Reqnroll  
**Supported .NET Versions:** .NET 6, .NET 8
