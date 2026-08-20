# CLAUDE.md

Guidance for Claude Code when working in this repository.

## Keep documentation in step with the code

**Before finishing any change, find the documents that describe what you touched and update
them in the same pull request.** When requirements change, update the document that stated
the old requirement rather than leaving it to be discovered later.

This is not housekeeping. This repository has a measured history of documentation that
described intent rather than behaviour, and it cost real time:

- `NAVIGATION.md`, `NAVIGATION_EXAMPLE.md`, `IMPLEMENTATION_SUMMARY.md` and `README.md` all
  described an automatic `PageName` → `PageNamePage` + `PageNameViewModel` convention. No
  such convention was ever implemented; registration is explicit. The gap hid the fact that
  the feature requested in #1 was not delivered.
- The add-a-page recipe omitted the required `.csproj` entry for months, so anyone following
  it produced a page that silently never compiled into the application.
- `NAVIGATION_EXAMPLE.md` claimed the navigation API was "type-safe — compile-time checking
  of page and view model types". It takes `System.Type`; nothing is checked at compile time.

A wrong document is worse than a missing one, because people act on it.

### Which document covers what

| If you change… | Update… |
|---|---|
| Navigation behaviour or the add-a-page flow | `NAVIGATION.md`, `NAVIGATION_EXAMPLE.md`, `IMPLEMENTATION_SUMMARY.md` |
| Project structure, dependencies, features, prerequisites | `README.md` |
| Build, test, style or PR process | `CONTRIBUTING.md` |
| An architectural choice worth recording | add an ADR under `docs/adr/` |
| Interaction-tracking events or fields | the data dictionary and the "what we capture" document (issues #112, #106) |
| A public API | its XML doc comments — this project documents public members |

### Also update the work tracker

When a story is finished, tick it in its epic's checklist and close the story issue. When
requirements change mid-flight, edit the issue body rather than leaving stale acceptance
criteria for the next person to follow.

## Verification: CI is the only thing that runs this code

There is **no .NET toolchain** (`dotnet`, `msbuild`, `mono`, `nuget`) on the machines where
changes are usually authored, so nothing can be built or tested locally. Verification is:

1. Push the branch and open a pull request against `master`.
2. Wait for **Build Debug** and **Build Release** to conclude — both run the NUnit suite.
3. Read failure logs, fix, push again.

A change is unverified until both jobs are green. Do not describe work as done before that.

CI cannot launch a WPF window, so **anything that only fails at runtime — DI wiring, XAML
resource resolution, whether a click does anything — still needs a manual run.** Say so
plainly rather than implying CI covered it.

## Traps specific to this codebase

- **`AutoLotManager.Desktop` is a legacy non-SDK project.** It does not glob source files.
  Every new `.cs` and `.xaml` needs a hand-written entry in
  `AutoLotManager.Desktop.csproj`. A `<Page>` entry without a matching `<Compile>` entry
  builds, navigates, and renders **blank** with no error anywhere. The other three projects
  are SDK-style and need no such entry.
- **Autofac supplies `ILifetimeScope` and `IComponentContext`, never `IContainer`.** Taking
  `IContainer` as a constructor dependency compiles and then throws at startup. This
  happened; see #50.
- **Add new registrations to the assertions in `AutoLotManager.Tests/BootstrapperTests.cs`**
  so a broken container fails in CI rather than in front of a user.
- `AutoLotManager.ViewModel` and `AutoLotManager.Core` target `netstandard2.0` and must stay
  free of WPF and UI dependencies — see #39 for what happens when UI packages leak in.

## Conventions

- **Branching:** trunk-based, short-lived branches, PR into `master`. See
  [`docs/adr/0001-branching-strategy.md`](docs/adr/0001-branching-strategy.md).
- **Style:** `.editorconfig` is authoritative — 4-space indent, Allman braces,
  `_camelCase` private fields, project usings before `System.*`.
- **Tests:** NUnit with `Assert.That` constraint syntax. Use `Throws.TypeOf` rather than
  `Throws.InstanceOf` where the exact exception type is the contract.
- Commit messages explain **why**, not just what.
