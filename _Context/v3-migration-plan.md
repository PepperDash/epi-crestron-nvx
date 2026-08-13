# v3 Migration Plan — epi-crestron-nvx

> Generated 2026-08-13 by essentials-epi agent (read-only `scan-plugin-v3-readiness.ps1`, Mode V3). Re-run the scan before starting work in case the repo has drifted since this was written.
> Work happens on branch `feature/v3-migration`.

## Program Context

This repo is 1 of 5 EPIs being converted to Essentials v3 in this effort:
`epi-videoCodec-ciscoExtended`, `epi-cisco-cli`, `epi-crestron-nvx`, `epi-epson-projector`, `epi-netgear-cli`.
`epi-videoCodec-ciscoExtended` is on hold until its in-progress feature branches are merged to `main` — do not start that one from this plan set.
**Do this repo last** — largest and most complex of the 5.

## Current State

| | |
|---|---|
| Target framework | `net472` |
| Essentials version | `2.28.0` |
| `SERIES4` define | present |
| C# files | 204 (largest of the 5) |
| `#if SERIES4` conditionals | 0 |
| Removed .NET APIs | **24 hits** — see breakdown below |
| Factories | 3 — `NvxApplicationFactory`, `NvxDirectorFactory` (both `minVersion: null` — never set), plus `NvxBaseDeviceFactory.cs` (base class, no `className`/`minVersion`/`typeNames` of its own — verify it doesn't need one) |
| 3-Series artifacts | none detected |
| Debug.Console remaining | 0 (404 calls already migrated to Serilog) |

## Risk: **High**

Largest file count by far, and — beyond the base v2→v3 mechanics — this repo **also needs the routing-interface migration** (confirmed below), not just the v2→v3 base workflow.

## ⚠️ Confirmed: Needs BOTH migrations

1. **Base v2→v3** (`migrate-v2-to-v3`) — csproj/net8, factory version bumps, logging already done.
2. **Routing-interface overhaul** (`migrate-plugin-routing-v3`) — confirmed via grep: this repo implements the **removed** interface family:
   - `NvxGlobalRouter : EssentialsDevice, IRoutingNumeric, IMatrixRouting` (`Features\Routing\NvxGlobalRouter.cs`)
   - `IRoutingInputSlot` implemented/used in `NvxMatrixClearInput.cs`, `NvxMatrixInput.cs`, `NvxMatrixOutput.cs`, `NvxMockMatrixInput.cs`, and stored in `Dictionary<string, IRoutingInputSlot>` collections in `NvxGlobalRouter.cs`

   Per [`context/routing-v3-migration.md`](../../../context/routing-v3-migration.md):
   - `IRoutingNumeric` / `IMatrixRouting` → **`IRoutingMidpointWithFeedback`**
   - `IRoutingInputSlot` → **no core equivalent** — since nothing outside this plugin appears to consume it, use the **plugin-local slot pattern** (a plugin-local `INvxInputSlot : IKeyName` interface carrying only the members actually used — see the skill for the template).
   - Reference the **epi-wyrestorm-networkHD PR #3** worked example in that doc — same shape (`IRoutingNumeric, IMatrixRouting` → `IRoutingMidpointWithFeedback`, plugin-local input slot).
   - This is a **design/architecture task**, not mechanical — budget real review time, and land as a **draft PR pending hardware/bench validation** (routing behavior isn't covered by validation test suites — see "Validation reality" in the routing doc).

## Removed .NET API Breakdown (24 hits — all mechanical, no design-discussion blockers)

| Pattern | Count | Fix |
|---|---|---|
| `eRoutingSignalType.SecondaryAudio` | ~14 | Replace with `eRoutingSignalType.AudioVideo` |
| `eRoutingSignalType.UsbInput` / `UsbOutput` | ~8 | Replace with `eRoutingSignalType.Usb` (⚠️ check for `UsbInput \| UsbOutput` combinations collapsing to `Usb \| Usb` — dedupe) |
| `Crestron.SimplSharp.Reflection` | 2 (`Enumeration.cs`, `AssemblyInfo.cs`) | `using System.Reflection;` |
| `.GetCType()` | 2 (`Enumeration.cs`) | Replace with `.GetType()` |

Files affected (from scan): `NvxMockDevice.cs`, `Enumeration.cs`, `NvxGlobalRouter.cs`, `NvxMatrixInput.cs`, `NvxMatrixOutput.cs`, `NvxMockMatrixInput.cs`, `PrimaryStreamRouter.cs`, `SecondaryAudioRouter.cs`, `UsbRouter.cs`, `AssemblyInfo.cs`, `InputPorts\SecondaryAudioInput.cs`, `InputSwitching\SwitcherForAnalogAudioOutput.cs`, `InputSwitching\SwitcherForSecondaryAudioOutput.cs`, `TieLines\TieLineConnector.cs`. None are "design-discussion" categories (no `BinaryFormatter`/`AppDomain.CreateDomain`/`Thread.Abort`/`System.Web`), so these don't block starting — but do them **before** the routing-interface work, since the interface migration touches the same routing files anyway.

## Skills to Use (in order)

1. [`sub-agents/essentials-epi/skills/migrate-v2-to-v3/SKILL.md`](../../../skills/migrate-v2-to-v3/SKILL.md) — Phases 1-2 (csproj, factory), then the removed-API fixes above as part of Phase 3
2. [`sub-agents/essentials-epi/skills/migrate-plugin-routing-v3/SKILL.md`](../../../skills/migrate-plugin-routing-v3/SKILL.md) — routing-interface overhaul
3. [`context/routing-v3-migration.md`](../../../context/routing-v3-migration.md) — reference doctrine for step 2

## Pre-Flight (do first, every session — per essentials-epi's mandatory git pre-flight rule)

1. `git fetch`, confirm branch is `feature/v3-migration`, confirm 0 behind upstream, confirm clean working tree.
2. Confirm this branch is still correctly based on an up-to-date `main`. Note this repo also had an in-progress feature branch (`feature/add-usb-audiovideo-switching`) at plan time — confirm with the user whether that work needs to land first, same concern as the codec repo.

## Task Checklist

- [ ] Re-run `analyze-plugin` scan to confirm nothing changed since this plan was written
- [ ] **Ask user**: does `feature/add-usb-audiovideo-switching` (or any other in-progress branch) need to merge to `main` before this migration starts? (Same pattern as the codec repo hold.)
- [ ] Phase 2a: `<TargetFramework>net472</TargetFramework>` → `net8` in `src\NvxEpi\NvxEpi.4Series.csproj`
- [ ] Phase 2a: remove both `SERIES4` `DefineConstants` `PropertyGroup` blocks
- [ ] Phase 2a: bump `PepperDashEssentials` PackageReference to `3.0.0`
- [ ] Phase 2b: set `MinimumEssentialsFrameworkVersion = "3.0.0"` on `NvxApplicationFactory` and `NvxDirectorFactory` (currently unset — not just outdated)
- [ ] Phase 2b: check `NvxBaseDeviceFactory.cs` — confirm whether it needs its own `MinimumEssentialsFrameworkVersion`/`TypeNames` or is purely a base class
- [ ] Phase 3: fix all 24 removed-API hits (table above) — `SecondaryAudio`→`AudioVideo`, `UsbInput`/`UsbOutput`→`Usb` (watch for redundant flag combos), `Crestron.SimplSharp.Reflection`→`System.Reflection`, `.GetCType()`→`.GetType()`
- [ ] Phase 3c: check `Initialize()`/`CustomActivate()` visibility and external callers (large device surface — check all device classes, not just one)
- [ ] **Routing migration**: convert `NvxGlobalRouter` (`IRoutingNumeric, IMatrixRouting`) to `IRoutingMidpointWithFeedback`; implement `ExecuteSwitch`/`ClearRoute`/`CurrentRoutes`/`RouteChanged`
- [ ] **Routing migration**: design + implement plugin-local `INvxInputSlot` (or similar) to replace `IRoutingInputSlot` across `NvxMatrixClearInput.cs`, `NvxMatrixInput.cs`, `NvxMatrixOutput.cs`, `NvxMockMatrixInput.cs`
- [ ] Phase 4: verify join maps — all fields `public`, `base(joinStart, typeof(...))`, `[JoinName]` attributes match
- [ ] Phase 5: `dotnet restore` + `dotnet build` on `NvxEpi.4Series.sln`, fix any compile errors
- [ ] Update README's "Minimum Essentials Framework Versions" section if present
- [ ] Commit with `feat!:` / `BREAKING CHANGE:` footer (major version bump)
- [ ] Verify `output/` `.cplz` builds
- [ ] **Land as draft PR** pending hardware/bench validation of routing behavior (build-clean ≠ routing-works, per routing doc's "Validation reality" section)

## Notes / Flags

- This is the highest-effort repo of the 5 — budget accordingly, and treat the routing-interface conversion as a design task requiring review, not a mechanical find/replace.
- Confirm with user whether the in-progress `feature/add-usb-audiovideo-switching` branch needs merging first (same open question as the codec repo).
