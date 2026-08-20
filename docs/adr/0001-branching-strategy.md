# ADR 0001: Branching strategy

- **Status:** Accepted
- **Date:** 2026-08-20

## Context

Until now this repository has had no written branching rule, and the cost showed up
concretely. Two agent-authored pull requests (#2, #4) were merged into a long-lived side
branch called `copilot-test-branch` rather than `master`. That branch then sat parallel to
the default branch for months, so `master` did not contain the navigation infrastructure,
the documentation, or — as it turned out — a compile error and a startup crash that nobody
had run into because nobody was building that code.

The repository has properties that make some strategies a poor fit:

- **One or two contributors**, plus agents working in parallel worktrees.
- **`AutoLotManager.Desktop.csproj` is a legacy non-SDK project** with no file globbing.
  Every `.cs` and `.xaml` is listed by hand in one `<ItemGroup>`, which makes that file a
  near-guaranteed conflict whenever two branches each add a Desktop file.
- **CI is the only verifier.** There is no .NET toolchain on the machines where changes are
  frequently authored, so a branch is unproven until it opens a pull request.
- Releases are not versioned or shipped on a cadence today.

## Decision

**Trunk-based development with short-lived branches.**

1. **`master` is the trunk.** It is the only long-lived branch and is always releasable.
   Nothing merges into any other shared branch.
2. **Every change happens on a short-lived branch** cut from the current `origin/master`,
   and returns via a pull request. Direct pushes to `master` are not made.
3. **Branches are measured in hours or days, not weeks.** A branch that cannot merge within
   about a week is too big and should be split.
4. **Naming:** `<type>/<issue-number>-<short-slug>`, for example
   `fix/65-tile-click-commands`, `feat/70-dbcontext-registration`,
   `docs/112-data-dictionary`, `chore/40-action-bumps`, `spike/69-orm-choice`.
   Types: `feat`, `fix`, `docs`, `chore`, `spike`, `test`.
5. **One issue per branch by default.** A single branch may cover several stories from the
   same epic **when they touch the same contended files** — the Desktop `.csproj`,
   `MainWindow.xaml`, `Bootstrapper.cs` and `NavigationConfiguration.cs` are the usual
   reason. Splitting those across branches produces conflicts, not isolation.
6. **Merge requires green CI** on both Debug and Release. CI is not advisory here; it is the
   only thing that has actually executed the code.
7. **Merge commits**, so the pull request and its discussion stay findable from history.
   Branches are deleted after merge.
8. **Rebase or merge `master` into a branch** that has fallen behind rather than letting it
   drift.

## Consequences

**Good**

- Work cannot accumulate off the default branch the way #2 and #4 did.
- Short branches mean the legacy csproj conflicts less often, and conflicts that do occur
  are small.
- `master` always reflects what is actually built and tested.
- Parallel agent work is safe as long as branches own disjoint files — which the naming and
  the one-issue rule make visible up front.

**Bad / accepted trade-offs**

- No release branch, so a hotfix to a shipped version is not supported. That is fine while
  nothing is shipped on a cadence; revisit if versioned releases begin, most likely by
  adding release branches cut from `master` rather than by adopting GitFlow.
- No `develop` branch, so `master` receives work continuously. This is only safe because CI
  gates every merge — which is why point 6 is not negotiable.

## Alternatives considered

- **GitFlow.** Rejected: `develop`, `release/*` and `hotfix/*` add ceremony that suits
  scheduled versioned releases. This project has none, and the failure mode it just
  experienced was precisely *too many long-lived branches*.
- **Direct commits to `master`.** Rejected: CI runs on pull requests, so committing straight
  to trunk skips the only verification available.
- **Keeping `copilot-test-branch` as an integration branch.** Rejected: this is the practice
  that caused the problem.
