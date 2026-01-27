# COPILOT README Generation Prompt

> **Quick Use:** Simply prompt: `Execute COPILOT_README_PROMPTS.md`

**Purpose:** Generate a production-ready README.md file for an Essentials plugin using SOURCE-FIRST EXTRACTION from COPILOT_README_PROMPTS.md refined methodology.

**Core Principle:** Source code is the single source of truth. Extract ALL data from source files (.cs, .csproj, .json). Existing README (if present) is optional verification only.

**Input Required (in priority order):**
1. Source code files (.cs files with properties, interfaces, methods)
2. Current framework version (from .csproj)
3. Config examples (from ExampleConfigFile.json or source)
4. Existing README.md (optional - for verification only)

**Output:** A comprehensive, developer-safe README following 6-phase source-first execution

**Works for:**
- Brand new plugins with no README
- Existing plugins (source code is authoritative)
- Auto-generation workflows

---

## MANDATORY SECTION ORDER & STRUCTURE

**The README.md MUST contain sections in this exact order (with HTML comment markers for each):**

1. `<!-- START Minimum Essentials Framework Versions -->` Title + Description + Framework version `<!-- END Minimum Essentials Framework Versions -->`
2. `<!-- START IMPORTANT -->` Device naming/safety warning `<!-- END IMPORTANT -->`
3. `<!-- START Supported Types -->` Device type list `<!-- END Supported Types -->`
4. `<!-- START Config Example -->` All config examples organized by type `<!-- END Config Example -->`
5. `<!-- START Device Series Capabilities -->` Table of device capabilities `<!-- END Device Series Capabilities -->`
6. `<!-- START Core Properties -->` Properties table `<!-- END Core Properties -->`
7. Property Details section (no markers - follows Core Properties directly)
8. `<!-- START Join Maps -->` Digital/Analog/Serial join tables `<!-- END Join Maps -->`
9. Join Details section (no markers - follows Join Maps directly)
10. `<!-- START Interfaces Implemented -->` List of interfaces `<!-- END Interfaces Implemented -->`
11. `<!-- START Base Classes -->` Organized by category (Device Base, Support) `<!-- END Base Classes -->`
12. `<!-- START Routing Framework -->` Routing architecture `<!-- END Routing Framework -->`
13. `<!-- START Configuration Best Practices -->` Best practices guidance `<!-- END Configuration Best Practices -->`

**ENFORCE:** If any section is missing or out of order, the README generation FAILS. Do not proceed without all sections in correct order.

---

## MANDATORY HEADING STRUCTURE (ALL EPIs ACROSS ALL PLUGINS)

**These headings MUST appear in README.md in this exact sequence:**

```
### ⚠️ IMPORTANT: Device Configuration Requirements
### Minimum Essentials Framework Versions
### Supported Types
### Config Examples
#### RS-232 Configuration (Serial Port) [if available in source]
#### TCP Configuration (Network) [if available in source]
#### IR Configuration (IR) [if available in source]
#### Multi-Zone Configuration [if available in source]
### Device Characteristics
### Core Properties
### Property Details
### Join Maps
#### Digitals
#### Analogs
#### Serials
### Join Details
### Interfaces Implemented
### Base Classes
### Routing Framework & Architecture
### Configuration Best Practices
### Bool Feedbacks
### Int Feedbacks
### String Feedbacks
### Public Methods
```

**EXTRACTION RULE:** 
- Extract ONLY config examples that exist in source (ExampleConfigFile.json, *Factory.cs, or *Device.cs). Do NOT invent config types.
- All other headings are MANDATORY for every plugin - do NOT skip them.
- Headings must appear in this exact order (sections 1-13 from MANDATORY SECTION ORDER above).
- Public Methods, Bool/Int/String Feedbacks may be integrated into Routing Framework OR kept as separate sections after Configuration Best Practices (per plugin needs)

---

## Quick Start (Automated Execution)

⚠️ **CRITICAL:** All phases must reference the **Reference Readme.md** embedded at end of this document (starts line ~627). Match the structure, depth, formatting, and content patterns EXACTLY.

### Phase 1: Foundation

**PROMPT 1.1 - Title, Framework Version, Critical Warning:**

```
Extract and create the introduction section:
### PROMPT 1.1 - Title, Framework Version, Critical Warning

```
Extract introduction from SOURCE CODE (primary) + verify against README (optional):

STEP 1 - Source Code Extraction:
1. READ: [.csproj] <Description> tag = Plugin description
2. READ: [.csproj] <PackageReference Include="PepperDashEssentials" Version="X.X.X">
3. READ: [*Device.cs] class comments and validation logic for warnings
4. READ: Any [Obsolete] or device-specific safety comments in code

STEP 2 - Optional README Verification:
5. IF [README.md] exists: Compare framework version and title
6. ALERT if discrepancy between source (.csproj) and README

CREATE: Introduction section:

# Crestron {PLUGIN_NAME} - Configuration Guide

{DESCRIPTION FROM CSPROJ}

### Minimum Essentials Framework Versions
- {VERSION FROM CSPROJ PACKAGEREFERENCE}

### ⚠️ IMPORTANT: {WARNING_TITLE}

{WARNING_TEXT FROM SOURCE CODE}

VALIDATION (SOURCE-FIRST):
✓ Title = From .csproj <AssemblyTitle> or <PackageId>
✓ Description = From .csproj <Description> tag
✓ Framework version = From PepperDashEssentials PackageReference
✓ Warning = From source code comments (not README)
✓ README.md (if exists) verified against source
```

**PROMPT 1.2 - Supported Types:**

```
Extract device types from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*Factory.cs] or [*Device.cs] supported device list
2. READ: Config files (ExampleConfigFile.json) for type examples
3. READ: Device initialization code for all supported types
4. EXTRACT: Complete list

STEP 2 - Optional README Verification:
5. IF [README.md] exists: Verify types match source
6. ALERT if README has extra/missing types

CREATE: Supported types section:

### Supported Types

**Device Types:**
- {TYPE_1} - {brief description from source}
- {TYPE_2} - {brief description from source}

VALIDATION (SOURCE-FIRST):
✓ Device names = From source code (Factory, Device class)
✓ All types from source are included
✓ README.md (if exists) matches source list
✓ No invented types
```

---

### Phase 2: Configuration Examples

**REFERENCE:** Look at the Config Examples section in the embedded Reference Readme (lines ~652-741). It shows:
- 3 examples: RS-232, TCP, Multi-Zone
- Each has title, complete JSON, and explanatory note
- All properties filled with real values
- Match this exact format for your output

**PROMPT 2.1 - Extract Configuration Examples (SOURCE-FIRST & USE REFERENCE README AS FORMAT GUIDE):**

```
CRITICAL RULE: Source code is authoritative. Extract examples from:
1. ExampleConfigFile.json (if exists) - PRIMARY
2. Config comments in *Factory.cs or *Device.cs - SECONDARY
3. Existing README.md - VERIFICATION ONLY

SOURCE EXTRACTION STEPS:
1. READ: [ExampleConfigFile.json] for COMPLETE config structure - extract ALL examples present
2. READ: [ExampleConfigFile.json] devices array - count ALL device configurations
3. IDENTIFY: ALL communication methods in examples (COM, TCP, SSH, IR, etc.)
4. READ: [*PropertiesConfig.cs] for all required/optional properties
5. READ: [*Factory.cs] comments describing config variations
6. READ: [*Device.cs] constructor to understand required properties
7. EXTRACT: ALL realistic config examples from source (one per communication method or mode)
8. NOTE: Do NOT invent scenarios - only extract what exists in source examples

EXAMPLE COUNT PRINCIPLE:
- Extract ALL communication method variations present in ExampleConfigFile.json
- For COM connections: Include comParams with actual baud rates, data bits, stop bits
- For TCP connections: Include tcpSshProperties with IP and port
- For plugins with multiple communication options: Include one example per method
- If ExampleConfigFile.json has N device examples: Extract all N (filtered by device type)
- **CRITICAL:** If source has both COM and TCP examples, both MUST be in README

STEP 2 - Optional README Verification:
9. IF [README.md] exists: Verify example count matches source file count
10. ALERT if examples missing or discrepancies between source and README

CREATE: Config examples with format:

### Config Example

#### {EXAMPLE_TITLE}

\`\`\`json
{COMPLETE_JSON_EXTRACTED_FROM_SOURCE}
\`\`\`
**Note:** {EXPLANATION_OF_THIS_CONFIG_VARIANT}

VALIDATION CHECKLIST (CRITICAL):
✓ All examples valid per source code structure
✓ Examples are realistic (not contrived scenarios)
✓ JSON is CORRECT (can be copy-pasted to production)
✓ All required properties from code included
✓ Optional properties shown in examples
✓ UIDs unique across examples
✓ Device types valid from Supported Types
✓ Properties match PropertiesConfig.cs exactly
✓ README.md (if exists) verified against source examples

DEVELOPER SAFETY GUARANTEE:
If a developer copy-pastes any config into system.json, device MUST initialize
without errors. This is non-negotiable.
```

VALIDATION CHECKLIST:
✓ All examples from README present (count varies by plugin)
✓ Examples in exact order as README
✓ JSON is COPIED not generated
✓ JSON formatting matches README exactly
✓ Property values match README exactly
✓ Notes are verbatim from README
✓ UIDs unique across examples
✓ Device types from Supported Types list
✓ No invented examples
✓ No missing examples
```

---

### Phase 3: Core Properties & Capabilities

**REFERENCE:** Look at the Core Properties section in the embedded Reference Readme (lines ~744-755). Shows:
- Table format with columns: Property, Type, Required, Default, Description
- All 9 properties listed (key, uid, name, type, group, control, pollTimeMs, warningTimeoutMs, errorTimeoutMs)
- Match this table structure exactly

**PROMPT 3.1 - Core Properties Table (SOURCE-FIRST, USE REFERENCE README FORMAT):**

```
Extract properties from SOURCE CODE (primary):

STEP 1 - Source Extraction:
1. READ: [*PropertiesConfig.cs] ALL [JsonProperty] decorated properties
2. READ: [*Device.cs] constructor to understand required vs optional
3. READ: Property validation logic for constraints
4. READ: Comments explaining each property

STEP 2 - Optional README Verification:
5. IF [README.md] has "Core Properties" section: Compare structure
6. ALERT if properties missing from README or README has invented ones

CREATE: Core Properties table:

### Core Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| \`{NAME}\` | {TYPE} | ✓/✗ | {DESCRIPTION} |

VALIDATION (SOURCE-FIRST):
✓ Properties = ALL from PropertiesConfig.cs (not subset)
✓ Types = Exact from [JsonProperty] declarations
✓ Required = Based on constructor logic, not guessed
✓ Description = From source code context
✓ All properties from source are included
✓ README.md (if exists) verified against source
```

---

### Phase 4: Property Details

**REFERENCE:** Look at the Property Details section in the embedded Reference Readme (lines ~758-835). Shows:
- Organized by sections (Core Configuration, Communication Configuration, Monitoring & Timeout)
- Each property with bold name and detailed explanation
- Real examples and ranges included
- Match this exact structure and depth

**PROMPT 4.1 - Detailed Property Explanations (SOURCE-FIRST, MATCH REFERENCE README DEPTH):**

```
Extract property documentation from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*PropertiesConfig.cs] ALL properties
2. READ: [*Device.cs] how properties are validated/used
3. READ: Constructor logic for constraints
4. READ: Validation methods for ranges/allowed values
5. GROUP: By functional category (Core, Communication, Timeouts, etc.)

STEP 2 - Optional README Verification:
6. IF [README.md] "Property Details" exists: Verify completeness
7. ALERT if README missing properties or has invented details

CREATE: Property explanations:

### Property Details

**{SECTION_NAME}:**

- **\`{PROPERTY}\`:** {EXPLANATION FROM SOURCE CONTEXT}

VALIDATION (SOURCE-FIRST):
✓ All table properties documented
✓ Explanations derived from source code
✓ Validation constraints explained
✓ Examples from actual config patterns
✓ README.md (if exists) verified against source
```

---

### Phase 5: Join Maps

**REFERENCE:** Look at the Join Maps section in the embedded Reference Readme (lines ~839-907). Shows:
- Three tables: Digitals (23 joins), Analogs (5 joins), Serials (4 joins)
- Columns: Join | Direction | Description
- Direction shows R (read), W (write), or R/W (bidirectional)
- Match this table format exactly and include ALL joins from source

**PROMPT 5.1 - Join Maps Tables (SOURCE-FIRST, MATCH REFERENCE README FORMAT & COMPLETENESS):**

```
Extract join definitions from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*JoinMap.cs] ALL public join definitions (int constants)
2. READ: [*Messenger.cs] join descriptions and enum values
3. EXTRACT: Digital, Analog, Serial joins in numerical order
4. Determine Direction: R (read-only), W (write-only), ↔ (bidirectional)

STEP 2 - Optional README Verification:
5. IF [README.md] "Join Maps" exists: Verify count and direction
6. ALERT if README has different join numbers or directions

CREATE: Join Maps:

### Join Maps

#### Digitals

| Join | Direction | Description |
|------|-----------|-------------|
| {NUMBER} | {R/W/↔} | {DESC} |

#### Analogs

| Join | Direction | Description |
|------|-----------|-------------|
| {NUMBER} | {R/W/↔} | {DESC} |

#### Serials

| Join | Direction | Description |
|------|-----------|-------------|
| {NUMBER} | {R/W/↔} | {DESC} |

VALIDATION (SOURCE-FIRST):
✓ Joins = ALL from [*JoinMap.cs] (complete list)
✓ Numbers = Exact from source (not estimated)
✓ Direction = Based on feedback/command logic
✓ Descriptions = From source code comments
✓ No duplicate joins
✓ Numbers in ascending order
✓ README.md (if exists) verified against source
```

**PROMPT 5.2 - Join Details (SOURCE-FIRST):**

```
Explain each join from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*Device.cs] feedback properties and methods
2. READ: [*Messenger.cs] for join explanations
3. GROUP: By function (Program Control, Status, Feedback, etc.)

STEP 2 - Optional README Verification:
4. IF [README.md] "Join Details" exists: Verify descriptions match
5. ALERT if README has different descriptions

CREATE: Join explanations:

### Join Details

**{CATEGORY_NAME}:**

- **{NUMBER} ({NAME}):** {EXPLANATION FROM SOURCE}
  {ENUM_VALUES if applicable}

VALIDATION (SOURCE-FIRST):
✓ All table joins documented
✓ Descriptions from source
✓ Enum values from source
✓ Grouping matches function
```

---

### Phase 6: Interfaces, Methods, Feedbacks, Best Practices

**REFERENCE:** Look at sections in the embedded Reference Readme (lines ~910+):
- Interfaces Implemented (line ~914): Lists 3 interfaces with descriptions
- Base Classes (line ~920): Organized into groups (Device Base, Support, Communication)
- Public Methods (line ~938): Organized by category (Program Control, LED, Timer Control, Time Adjustment, Communication)
- Bool/Int/String Feedbacks (lines ~968+): Simple bullet lists with descriptions
- Configuration Best Practices (lines ~987+): 6 subsections with detailed guidance
- Production Deployment: 8+ validation steps
- Troubleshooting: 6+ common issue/solution pairs
- Match ALL these sections exactly in structure, depth, and content organization

**PROMPT 6.1 - Interfaces & Base Classes (SOURCE-FIRST, REFERENCE README FORMAT):**

```
Extract complete architecture from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*Device.cs] class declaration (e.g., "class X : BaseClass, IInterface1, IInterface2")
2. READ: [*JoinMap.cs] class declaration for support base classes
3. READ: [*Messenger.cs] class declaration for messaging base classes
4. READ: Private fields for communication interfaces
5. EXTRACT: All direct inheritance and interfaces

STEP 2 - Optional README Verification:
6. IF [README.md] "Interfaces" or "Base Classes" exists: Verify against source
7. ALERT if README missing classes or has invented ones

CREATE: Architecture sections:

### Interfaces Implemented

- \`{InterfaceName}\` - {Brief description}

### Base Classes

**Device Base Classes:**
- \`{ClassName}\` - {Purpose}

**Support Classes:**
- \`{SupportClassName}\` - {Purpose} (base for specific implementation)

**Communication & Monitoring:**
- \`{InterfaceOrClassName}\` - {Purpose}

VALIDATION (SOURCE-FIRST):
✓ Interfaces = From [*Device.cs] "class X : ... , IInterface" syntax
✓ Device base class = From [*Device.cs] direct parent
✓ Support classes = From [*JoinMap.cs], [*Messenger.cs] declarations
✓ Communication = From private readonly fields
✓ All inherited from source (no README-only items)
✓ README.md (if exists) verified against source
```

**PROMPT 6.2 - Public Methods (SOURCE-FIRST):**

```
Extract methods from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*Device.cs] ALL public methods
2. CATEGORIZE: By functional purpose
3. EXTRACT: Method name, parameters, purpose from context

STEP 2 - Optional README Verification:
4. IF [README.md] "Public Methods" exists: Verify count
5. ALERT if methods missing or invented in README

CREATE: Methods section:

### Public Methods

**{CATEGORY}:**
- \`{MethodName}()\` - {Purpose}

VALIDATION (SOURCE-FIRST):
✓ Methods = ALL public from source
✓ Signatures = Exact from [*Device.cs]
✓ Descriptions = From code context/comments
✓ Categorized logically by function
✓ README.md (if exists) verified against source
```

**PROMPT 6.3 - Device Feedbacks (SOURCE-FIRST):**

```
Extract feedback objects from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*Device.cs] ALL public Feedback properties
2. CATEGORIZE: BoolFeedback, IntFeedback, StringFeedback
3. EXTRACT: Property name, value ranges, purpose

STEP 2 - Optional README Verification:
4. IF [README.md] feedback sections exist: Verify completeness
5. ALERT if feedbacks missing or invented in README

CREATE: Feedbacks section:

### Device Feedbacks

**Bool Feedbacks (Digital):**
- \`{FeedbackName}\` - {Description}

**Int Feedbacks (Analog):**
- \`{FeedbackName}\` - {Description} ({VALUE_RANGE})

**String Feedbacks (Serial):**
- \`{FeedbackName}\` - {Description}

VALIDATION (SOURCE-FIRST):
✓ Feedbacks = ALL from source [*Device.cs] properties
✓ Names = Exact property names
✓ Descriptions = From code context
✓ Value ranges = For Int feedbacks
✓ README.md (if exists) verified against source
```

**PROMPT 6.4 - Configuration Best Practices (SOURCE-FIRST):**

```
Extract operational guidance from SOURCE CODE:

STEP 1 - Source Extraction:
1. READ: [*Device.cs] constructor validation logic
2. READ: Communication setup code for constraints
3. READ: Comments explaining intended usage
4. EXTRACT: Network, timeout, property, monitoring patterns

STEP 2 - Derive from Examples:
5. Extract realistic scenarios from [ExampleConfigFile.json]
6. Derive best practices from config patterns

STEP 3 - Optional README Verification:
7. IF [README.md] "Best Practices" exists: Verify accuracy
8. ALERT if practices contradict source validation logic

CREATE: Best Practices section:

### Configuration Best Practices

**{CATEGORY}:**
- {Guideline derived from source logic}
- {Guideline derived from source logic}

VALIDATION (SOURCE-FIRST):
✓ Practices = Derived from source code validation logic
✓ Practices = Consistent with config examples
✓ Practices = Production-safe (can be copy-pasted)
✓ Typical pitfalls = Based on validation constraints
✓ README.md (if exists) verified against source
```

---

## Execution Checklist

**🚨 BEFORE STARTING ANY PHASE:**
1. **Scroll to end of this document** (~line 627+)
2. **Read the "Reference Readme.md"** embedded here - this IS your format guide
3. Use it as the template for all output structure, depth, and content patterns
4. All config examples in Reference show the exact format and completeness expected
5. All property details in Reference show the depth and organization expected

---

**Before Generating (Source-First):**
- [ ] Source code files readable (*Device.cs, *PropertiesConfig.cs, *JoinMap.cs, *Messenger.cs)
- [ ] .csproj file accessible for framework version
- [ ] ExampleConfigFile.json present (if applicable)
- [ ] README.md exists or is empty (optional for verification)

**During Generation (All Phases):**
- [ ] Phase 1: Title/Framework/Warning extracted from source
- [ ] Phase 1: Supported Types extracted from source code
- [ ] Phase 2: Config examples generated from source structure
- [ ] Phase 3: Core Properties extracted from PropertiesConfig.cs
- [ ] Phase 4: Property Details derived from source logic
- [ ] Phase 5: Join Maps extracted from *JoinMap.cs
- [ ] Phase 5: Join Details from source code
- [ ] Phase 6: Interfaces/Base Classes from source declarations
- [ ] Phase 6: Public Methods extracted
- [ ] Phase 6: Feedbacks documented
- [ ] Phase 6: Methods extracted
- [ ] Phase 6: Feedbacks documented
- [ ] Phase 6: Best Practices derived from source logic

**Quality Validation (Source-First):**
- [ ] ALL data extracted from source code (not README)
- [ ] Config examples are valid per PropertiesConfig.cs
- [ ] Properties match PropertiesConfig.cs exactly
- [ ] Joins match *JoinMap.cs exactly
- [ ] Interfaces match [*Device.cs] class declaration
- [ ] Base classes match inheritance hierarchy
- [ ] Methods match [*Device.cs] public methods
- [ ] Feedbacks match [*Device.cs] properties
- [ ] No invented content
- [ ] README.md (if exists) verified against source for discrepancies

**Final Output:**
- [ ] README.md ~600-800 lines
- [ ] All 6 phases complete
- [ ] Production-ready
- [ ] Developer copy-paste safe
- [ ] 100% sourced from code, not assumptions

---

## Reference Implementation Mode (CRITICAL)

**ECOSYSTEM STANDARD: All EPI plugin READMEs must follow the embedded Reference Readme depth and structure.**

**IF README.md already exists in the plugin directory OR this is a new plugin:**

1. **Reference the Embedded Reference Readme** (at end of this file) as the ONLY mandatory style/depth template
2. **Match these sections** in order and depth:
   - Multiple config examples covering all device types/modes
   - Device capabilities/series comparison table (if multiple device types exist)
   - Core properties table with TX/RX columns (if applicable)
   - Property details with device-specific subsections
   - Join maps (digital, analog, serial)
   - Join details categorized by function
   - Interfaces implemented
   - Base classes
   - Routing/architectural framework (if applicable)
   - Configuration best practices with production guidance

3. **Config Examples Requirement:**
   - Minimum: 1 example per device type OR communication method
   - NVX has 12+ examples showing all variations
   - For Limitimer: Include RS-232 + TCP + any other modes
   - Each example must be copy-paste production-safe

4. **Property Details Requirement:**
   - Core configuration section
   - Device-specific sections (TX-only, RX-only, etc.)
   - Detailed constraints and validation rules
   - Real values from actual source files, not generic descriptions

5. **Best Practices Requirement:**
   - Multiple subsections (Device Naming, Device IDs, Timeouts, Validation, etc.)
   - Specific guidance derived from source validation logic
   - Production deployment safety checks
   - Common pitfalls and how to avoid them

**Why this matters:** ALL READMEs across this plugin ecosystem must look professional and comprehensive. Minimal READMEs are not acceptable.

**Red flags (STOP and regenerate if detected):**
- ❌ Fewer than 3 config examples (unless single device type)
- ❌ Property details missing device-specific sections
- ❌ No capabilities/series comparison table (if multiple types)
- ❌ Best practices section too brief or generic
- ❌ Missing architectural/routing framework section (if applicable)
- ❌ Join details not categorized by function
- ❌ Config examples not production-safe (missing constraints)

---

## Reference Implementation Mode (OLD)

**IF README.md already exists in the plugin directory:**

1. **Load existing README as the style template** — Do NOT generate from scratch
2. **Extract style signature** — Note: section count, example count, table formats, verbosity level, HTML comment boundaries
3. **Regenerate content only** — Update facts and source data while preserving structure and formatting exactly
4. **Validate style match** — Compare generated output against existing README section-by-section
5. **Output only if style matches** — If divergence detected, regenerate with matching style

**Why this matters:** ALL READMEs across this plugin ecosystem must look identical in structure, organization, and tone. Consistency is the mandate, not just the 6-phase structure.

**Red flags (STOP and regenerate if detected):**
- ❌ Adding new sections not in existing README
- ❌ Changing section order from existing README
- ❌ Making content significantly more verbose than existing README
- ❌ Using different example count than existing README
- ❌ Removing HTML comment boundaries (`<!-- START -->` / `<!-- END -->`)
- ❌ Changing table formats or styling
- ❌ Different tone or voice from existing README

**Validation checklist against existing README:**
- [ ] Same section titles in same order?
- [ ] Same number of config examples?
- [ ] Same table formats (column count, structure)?
- [ ] Verbosity level matches (concise vs. detailed)?
- [ ] HTML comment markers preserved?
- [ ] Example scenarios cover same device types?
- [ ] Device descriptions equally detailed?

**Example:** If existing README has 9 config examples → generate exactly 9. If it has tables with 4 columns → use 4 columns, not 5.

---

## Key Principles

1. **SOURCE-FIRST EXTRACTION** - All data from .cs, .csproj, .json files. README is optional verification.
2. **EXACT MATCH** - Property names, join numbers, method signatures match source exactly.
3. **DEVELOPER-SAFE** - Every config example must copy-paste and initialize without errors.
4. **AUTHORITATIVE SOURCE** - Source code is single source of truth; README confirms or alerts.
5. **NO INVENTION** - Zero generated content. Everything extracted or derived from source logic.
6. **STYLE CONSISTENCY** - Existing README (if present) is the mandatory style template. Preserve structure, organization, and tone exactly.

---

## Usage

1. **Save this prompt** as COPILOT_README.md
2. **Copy entire prompt** into Copilot/Claude
3. **Provide context**: Plugin name, framework version, source files
4. **Execute phases 1-6 sequentially**
5. **Validate** against checklist
6. **Output**: Production-ready README.md

---

**Template Version:** 2.0 (REFINED PROMPTS)  
**Created:** January 27, 2026  
**Based On:** COPILOT_README_PROMPTS.md


## Reference Readme.md starts here:
# Crestron Dsan Limitimer - Configuration Guide

Comprehensive timer control and program management for Dsan Limitimer devices. Integrates with Crestron Essentials for LED feedback, time adjustment, program selection, and communication monitoring.

### ⚠️ IMPORTANT: Device Configuration Requirements

The configuration properties must accurately reflect the communication parameters and timeout values required for proper device communication. Incorrect timeout values may result in communication errors or device initialization failures. All timeouts must be set appropriately for your network environment and device responsiveness requirements.

---
<!-- START Minimum Essentials Framework Versions -->
### Minimum Essentials Framework Versions

- 2.8.0
<!-- END Minimum Essentials Framework Versions -->

<!-- START Supported Types -->
### Supported Types

**Devices:**
- `limitimer` - DSAN Limitimer timer control device (RS-232 and TCP/Network)
<!-- END Supported Types -->

---

<!-- START Config Example -->
### Config Examples

#### RS-232 Configuration (Serial Port)

```json
{
  "key": "limitimer-1",
  "name": "MainTimer",
  "type": "limitimer",
  "group": "timers",
  "uid": 101,
  "properties": {
    "control": {
      "comParams": {
        "dataBits": 8,
        "softwareHandshake": "None",
        "baudRate": 9600,
        "parity": "None",
        "stopBits": 1,
        "hardwareHandshake": "None",
        "protocol": "RS232"
      },
      "method": "com",
      "controlPortNumber": 1
    },
    "pollTimeMs": 5000,
    "warningTimeoutMs": 45000,
    "errorTimeoutMs": 90000
  }
}
```
**Note:** Standard RS-232 configuration for Limitimer devices. Set `controlPortNumber` to the COM port on your control processor (typically 1-3 for CP4). Serial communication at 9600 baud with standard RS-232 parameters (8 data bits, no parity, 1 stop bit).

#### TCP Configuration (Network)

```json
{
  "key": "limitimer-network",
  "name": "NetworkTimer",
  "type": "limitimer",
  "group": "timers",
  "uid": 102,
  "properties": {
    "control": {
      "tcpSshProperties": {
        "address": "192.168.1.50",
        "port": 5000
      }
    },
    "pollTimeMs": 5000,
    "warningTimeoutMs": 45000,
    "errorTimeoutMs": 90000
  }
}
```
**Note:** TCP/Network configuration for Limitimer devices with ethernet connectivity. Requires static IP address and network accessibility. Port 5000 is the standard Limitimer TCP port.

#### Multi-Zone Configuration

```json
{
  "key": "limitimer-zone-1",
  "name": "Zone-1-Timer",
  "type": "limitimer",
  "group": "timers",
  "uid": 110,
  "properties": {
    "control": {
      "comParams": {
        "dataBits": 8,
        "softwareHandshake": "None",
        "baudRate": 9600,
        "parity": "None",
        "stopBits": 1,
        "hardwareHandshake": "None",
        "protocol": "RS232"
      },
      "method": "com",
      "controlPortNumber": 1
    },
    "pollTimeMs": 3000,
    "warningTimeoutMs": 30000,
    "errorTimeoutMs": 60000
  }
}
```
**Note:** Multi-zone setup with faster polling (3000ms). Appropriate for high-traffic environments with multiple timers. Reduces timeouts for faster error detection.

<!-- END Config Example -->

---

### Device Characteristics

| Feature | Support | Notes |
|---------|---------|-------|
| Communication | RS-232, TCP | Primary: RS-232 at 9600 baud |
| LED Indicators | 7 total | Program 1-3, Session, Beep, Blink, Seconds |
| Time Display | 3 zones | Total, Sum-Up, Remaining (MM:SS format) |
| Programs | 4 | Program 1-3 + Session mode |
| Controls | 6 | Start/Stop, Repeat, Clear, +/-, Seconds |
| Feedback Status | Online, Communication Health | Real-time monitoring |

---

<!-- START Core Properties -->
### Core Properties

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `key` | string | ✓ | - | Unique device identifier |
| `uid` | integer | ✓ | - | Essentials system UID (must be unique) |
| `name` | string | ✓ | - | Device display name |
| `type` | string | ✓ | - | Device type (always "limitimer") |
| `group` | string | ✓ | - | Device grouping (e.g., "timers") |
| `control` | object | ✓ | - | Communication control configuration |
| `pollTimeMs` | long | ✗ | 5000 | Status poll interval in milliseconds |
| `warningTimeoutMs` | long | ✗ | 45000 | Communication warning threshold in ms |
| `errorTimeoutMs` | long | ✗ | 90000 | Communication error threshold in ms |

<!-- END Core Properties -->

---

### Property Details

**Core Configuration:**

- **`key`:** Unique device identifier within the system. Used for device reference in routing and control logic. Example: `"limitimer-main"`, `"limitimer-backup"`.

- **`uid`:** Essentials system UID. Must be unique across all devices. Critical for device communication and feedback routing. Range: 1-65535.

- **`name`:** Device name displayed in the control system UI and used internally. Should be descriptive (e.g., `"MainTimer"`, `"ConferenceRoomTimer"`).

- **`type`:** Device type identifier. Must be set to `"limitimer"` for all Limitimer devices.

- **`group`:** Logical grouping category for device organization. Recommended: `"timers"` or `"presentation"`.

**Communication Configuration:**

- **`control`:** Communication control object. Contains either:
  - **COM (Serial):** `comParams` with baudRate (9600), dataBits (8), parity (None), stopBits (1), protocol (RS232)
  - **TCP (Network):** `tcpSshProperties` with IP address and port (5000)

- **`control.method`:** Communication method:
  - `"com"` for RS-232 serial connection
  - `"tcp"` for TCP/Network connection

- **`control.controlPortNumber`:** (COM only) Control processor's serial port number (typically 1-3 for CP4).

- **`control.comParams.baudRate`:** Serial communication speed. Must be 9600 for Limitimer devices (fixed by device firmware).

- **`control.comParams.dataBits`:** Serial data bits. Must be 8 for Limitimer.

- **`control.comParams.stopBits`:** Serial stop bits. Must be 1 for Limitimer.

- **`control.comParams.parity`:** Serial parity. Must be "None" for Limitimer.

- **`control.tcpSshProperties.address`:** (TCP only) Device IP address. Must be static for reliable connectivity.

- **`control.tcpSshProperties.port`:** (TCP only) Device port. Standard Limitimer TCP port is 5000.

**Monitoring & Timeout:**

- **`pollTimeMs`:** How often (in ms) the system queries device status. 
  - **Recommended:** 5000 ms (5 seconds) for normal operation
  - **Range:** 1000-60000 ms
  - **Lower values:** More responsive but higher network traffic
  - **Higher values:** Less traffic but slower status updates
  - **Multi-zone:** Use 3000 ms for high-traffic environments

- **`warningTimeoutMs`:** Time (in ms) before communication warning is triggered. 
  - **Recommended:** 45000 ms (45 seconds) for standard networks
  - **Calculation:** Should be at least 3x pollTimeMs + network latency
  - **Purpose:** Early warning of communication degradation
  - **Example:** 5000 ms polling × 3 + 30 ms latency = ~15 s minimum (45 s recommended)

- **`errorTimeoutMs`:** Time (in ms) before communication error is triggered. 
  - **Recommended:** 90000 ms (90 seconds) for standard networks
  - **Calculation:** Should be at least 2x warningTimeoutMs
  - **Purpose:** Device marked offline and removed from control
  - **Example:** 45000 ms warning × 2 = 90000 ms error threshold
  - **Note:** Should be significantly larger than warningTimeoutMs to prevent false errors

---
<!-- START Join Maps -->
### Join Maps

#### Digitals

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | R | Is Online |
| 11 | R/W | Program 1 Press / Program 1 LED On Feedback |
| 12 | R | Program 1 LED Dim Feedback |
| 13 | R/W | Program 2 Press / Program 2 LED On Feedback |
| 14 | R | Program 2 LED Dim Feedback |
| 15 | R/W | Program 3 Press / Program 3 LED On Feedback |
| 16 | R | Program 3 LED Dim Feedback |
| 17 | R/W | Session Press / Session LED On Feedback |
| 18 | R | Session LED Dim Feedback |
| 21 | R/W | Beep Press / Beep LED On Feedback |
| 22 | R/W | Blink Press / Blink LED On Feedback |
| 23 | R/W | Seconds Mode Press / Seconds Mode Indicator Feedback |
| 24 | R | Green LED On Feedback |
| 25 | R | Red LED On Feedback |
| 26 | R | Yellow LED On Feedback |
| 27 | W | Start/Stop Press |
| 28 | W | Repeat Press |
| 29 | W | Clear Press |
| 30 | W | Total Time Plus Press |
| 31 | W | Total Time Minus Press |
| 32 | W | Sum Time Plus Press |
| 33 | W | Sum Time Minus Press |

#### Analogs

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | R | Socket Status (0=IsOk, 1=CompromisedCommunication, 2=CommunicationError) |
| 2 | R | Program 1 LED State (0=off, 1=on, 2=dim) |
| 3 | R | Program 2 LED State (0=off, 1=on, 2=dim) |
| 4 | R | Program 3 LED State (0=off, 1=on, 2=dim) |
| 5 | R | Session LED State (0=off, 1=on, 2=dim) |

#### Serials

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | R | Device Name |
| 2 | R | Total Time String (MM:SS format) |
| 3 | R | Sum-Up Time String (MM:SS format) |
| 4 | R | Remaining Time String (MM:SS format) |
<!-- END Join Maps -->

---

### Join Details

**Digital Joins - Status & Feedback:**

- **1 (Is Online):** Device online status feedback. True = online/responding, False = offline/no communication.

**Digital Joins - Program Control:**

- **11-12 (Program 1):** Program 1 selection feedback and LED states (on/dim). Bidirectional press control.
- **13-14 (Program 2):** Program 2 selection feedback and LED states (on/dim). Bidirectional press control.
- **15-16 (Program 3):** Program 3 selection feedback and LED states (on/dim). Bidirectional press control.
- **17-18 (Session):** Session mode selection feedback and LED states (on/dim). Bidirectional press control.

**Digital Joins - Control Functions:**

- **21 (Beep):** Beep button press and LED feedback. Toggles beep function on/off.
- **22 (Blink):** Blink button press and LED feedback. Toggles blink function on/off.
- **23 (Seconds Mode):** Seconds mode toggle and indicator feedback. Changes time display format.
- **27 (Start/Stop):** Timer start/stop control (write-only). High pulse starts timer, low pulse stops timer.
- **28 (Repeat):** Repeat function control (write-only). Re-runs current program.
- **29 (Clear):** Clear timer control (write-only). Resets all timer values.

**Digital Joins - Time Adjustment:**

- **30-31 (Total Time):** Increment/decrement total time. Joins 30 (plus) and 31 (minus) adjust total time value.
- **32-33 (Sum Time):** Increment/decrement sum-up time. Joins 32 (plus) and 33 (minus) adjust sum time value.

**Digital Joins - LED Status:**

- **24 (Green LED):** Green LED state feedback (read-only). Indicates normal/ready status.
- **25 (Red LED):** Red LED state feedback (read-only). Indicates warning/error status.
- **26 (Yellow LED):** Yellow LED state feedback (read-only). Indicates caution/attention needed.

**Analog Joins - State Tracking:**

- **1 (Socket Status):** Communication socket status (read-only). Values: 0=Ok, 1=CompromisedCommunication (warning), 2=CommunicationError (offline).
- **2-5 (LED States):** LED state values (read-only). Values: 0=off, 1=on, 2=dim. One join per program LED.

**Serial Joins - Data Feedback:**

- **1 (Device Name):** Device name string feedback (read-only). Returns configured device name.
- **2 (Total Time String):** Total elapsed time in MM:SS format (read-only). Format: "HH:MM:SS".
- **3 (Sum-Up Time String):** Sum-up time in MM:SS format (read-only). Format: "HH:MM:SS".
- **4 (Remaining Time String):** Remaining time in MM:SS format (read-only). Format: "HH:MM:SS".

---
<!-- START Interfaces Implemented -->
### Interfaces Implemented

- `IOnline` - Provides online/offline status feedback
- `ICommunicationMonitor` - Enables communication status monitoring and diagnostics
- `IBridgeAdvanced` - Supports advanced bridge functionality for API linking
<!-- END Interfaces Implemented -->

---

<!-- START Base Classes -->
### Base Classes

**Device Base Classes:**
- `EssentialsBridgeableDevice` - Bridgeable Essentials device implementation

**Support Classes:**
- `JoinMapBaseAdvanced` - Advanced join mapping framework (base for LimitimerBridgeJoinMap)
- `MessengerBase` - Message handling and communication base class (base for LimitimerMessenger)

**Communication & Monitoring:**
- `IBasicCommunication` - Basic communication interface for device control
- `GenericCommunicationMonitor` - Communication monitoring and status tracking
<!-- END Base Classes -->

---

<!-- START Public Methods -->
### Public Methods

**Program Control Methods:**
- `Program1()` - Trigger Program 1
- `Program2()` - Trigger Program 2
- `Program3()` - Trigger Program 3
- `Session4()` - Trigger Session function

**LED & Indicator Methods:**
- `Beep()` - Trigger beep with LED feedback
- `Beep1()` - Alternate beep variant
- `Blink()` - Trigger blink function

**Timer Control Methods:**
- `StartStop()` - Start or stop the timer
- `Repeat()` - Repeat the current timer program
- `Clear()` - Clear the timer

**Time Adjustment Methods:**
- `TotalTimePlus()` - Increment total time
- `TotalTimeMinus()` - Decrement total time
- `SumTimePlus()` - Increment sum-up time
- `SumTimeMinus()` - Decrement sum-up time
- `SetSeconds()` - Set timer to seconds mode

**Communication Methods:**
- `ProcessFeedbackMessage(string message)` - Process device feedback messages
- `SendText(string text)` - Send text command to device
- `LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)` - Link device to Essentials API
<!-- END Public Methods -->
---

<!-- START Bool Feedbacks -->
### Bool Feedbacks

- `IsOnline` - Device online status
- `BeepLedStateFeedback` - Beep LED state
- `BlinkLedStateFeedback` - Blink LED state
- `GreenLedStateFeedback` - Green LED state
- `RedLedStateFeedback` - Red LED state
- `YellowLedStateFeedback` - Yellow LED state
- `SecondsModeIndicatorStateFeedback` - Seconds mode indicator state
<!-- END Bool Feedbacks -->

---

<!-- START Int Feedbacks -->
### Int Feedbacks

- `StatusFeedback` - Communication socket status (0=Ok, 1=Warning, 2=Error)
- `Program1LedStateFeedback` - Program 1 LED state (0=off, 1=on, 2=dim)
- `Program2LedStateFeedback` - Program 2 LED state (0=off, 1=on, 2=dim)
- `Program3LedStateFeedback` - Program 3 LED state (0=off, 1=on, 2=dim)
- `SessionLedStateFeedback` - Session LED state (0=off, 1=on, 2=dim)
<!-- END Int Feedbacks -->

---

<!-- START String Feedbacks -->
### String Feedbacks

- `TotalTimeFeedback` - Total elapsed time in MM:SS format
- `SumUpTimeFeedback` - Sum-up time in MM:SS format
- `RemainingTimeFeedback` - Remaining time in MM:SS format
<!-- END String Feedbacks -->

---

<!-- START Configuration Best Practices -->
### Configuration Best Practices

**Communication Setup:**
- Verify Limitimer device is powered and initialized before deployment
- For RS-232: Test COM port connectivity using terminal utility before final config
- For RS-232: Ensure baud rate is set to 9600 on device (device firmware fixed)
- For TCP: Verify device IP address is static and documented
- For TCP: Verify firewall rules allow port 5000 access from control processor
- Test ping/connectivity from CP4 to device before deployment

**Timeout Configuration:**
- **Standard Network (5000ms polling):** 
  - `warningTimeoutMs`: 45000 (45 seconds)
  - `errorTimeoutMs`: 90000 (90 seconds)
- **High-Traffic Environments (3000ms polling):**
  - `warningTimeoutMs`: 30000 (30 seconds)
  - `errorTimeoutMs`: 60000 (60 seconds)
- **Low-Latency Requirements (2000ms polling):**
  - `warningTimeoutMs`: 15000 (15 seconds)
  - `errorTimeoutMs`: 30000 (30 seconds)
- Always ensure: `errorTimeoutMs` ≥ 2 × `warningTimeoutMs`
- Never set `pollTimeMs` lower than 1000 ms (1 second)

**Device Management:**
- Use unique, descriptive device keys (e.g., "limitimer-main", "limitimer-backup")
- Assign UIDs sequentially for easy tracking (101, 102, 103, etc.)
- Group devices logically using the `group` property (e.g., "timers", "presentation", "conference")
- Document device location and purpose for maintenance
- Maintain IP address documentation for TCP configurations

**Monitoring & Debugging:**
- Enable communication monitoring to track device status
- Check `StatusFeedback` (Analog Join 1) for real-time connection health
- Monitor `IsOnline` feedback for device availability
- Review logs if devices frequently enter warning or error state
- Verify timeout values if communication issues occur
- Use Serial Join 1 (Device Name) to confirm device identity
- Monitor Serial Joins 2-4 for time display accuracy

**Production Deployment:**
- Test configuration in staging environment first
- Verify all program buttons (1-3, Session) trigger expected actions
- Confirm LED feedback states (green, red, yellow) update correctly

---

## FINAL README STRUCTURE VALIDATION CHECKLIST

**Before committing any README.md, verify ALL of the following:**

### Section Presence & Order (REQUIRED)
- [ ] Section 1: Framework Versions (with HTML markers)
- [ ] Section 2: Device Naming Warning (with HTML markers)
- [ ] Section 3: Supported Types (with HTML markers)
- [ ] Section 4: Config Examples (with HTML markers)
- [ ] Section 5: Device Series Capabilities (with HTML markers)
- [ ] Section 6: Core Properties Table (with HTML markers)
- [ ] Section 7: Property Details (no markers)
- [ ] Section 8: Join Maps - Digitals/Analogs/Serials (with HTML markers)
- [ ] Section 9: Join Details (no markers)
- [ ] Section 10: Interfaces Implemented (with HTML markers)
- [ ] Section 11: Base Classes (with HTML markers, organized by category)
- [ ] Section 12: Routing Framework (with HTML markers)
- [ ] Section 13: Configuration Best Practices (with HTML markers)
- [ ] Sections appear in this exact order (no reordering)

### HTML Comment Markers (REQUIRED)
- [ ] All sections with markers have matching START/END pairs
- [ ] No orphaned START or END markers
- [ ] Marker format: `<!-- START SectionName -->` and `<!-- END SectionName -->`
- [ ] No extra content between markers and section content

### Content Accuracy (REQUIRED)
- [ ] Framework version matches .csproj PackageReference (not README)
- [ ] All device types from source Factory code included (no invented types)
- [ ] All join numbers match JoinMap.cs exactly
- [ ] Join directions (R/W/↔) match JoinCapabilities from source
- [ ] All config examples are from source (not invented)
- [ ] No developer safety issues (configs are production-ready)
- [ ] Property details match source code validation logic

### Formatting Consistency (REQUIRED)
- [ ] Tables use consistent markdown format (pipes, dashes)
- [ ] Code blocks use triple backticks (```)
- [ ] JSON formatting is valid and properly indented
- [ ] Headings use correct markdown levels (# ## ### etc.)
- [ ] Bold text uses ** around content
- [ ] Links are properly formatted
- [ ] Lists use consistent bullet format (- for unordered)

### Base Classes Organization (REQUIRED)
- [ ] Base Classes section organized into subsections
- [ ] Examples: "Device Base:", "Support Classes:", "Communication & Monitoring:"
- [ ] Each class/interface has a descriptive comment
- [ ] Subsections are grouped logically by function

### FAILURE CONDITIONS (Stop processing if ANY present)
- ❌ Missing any required section from the 13-section list
- ❌ Sections out of order
- ❌ HTML markers mismatched or orphaned
- ❌ Join numbers do not match JoinMap.cs source
- ❌ Device types include 385/385C (commented out in NvxBaseDeviceFactory.cs)
- ❌ Framework version is not 2.24.4
- ❌ Config examples invented rather than extracted from source
- ❌ Base Classes not organized into subsections with descriptions
- Test time adjustment (plus/minus) for both total and sum time
- Verify start/stop, repeat, and clear functions work reliably
- Confirm communication status feedback updates appropriately
- Perform full system test with network load before go-live
- Document all configuration values (UIDs, IPIDs, IP addresses)

**Troubleshooting:**
- If device goes offline: Check network connectivity, firewall rules, and timeout settings
- If feedback delays: Lower polling interval (3000ms or less) or increase timeouts
- If LED states inconsistent: Verify correct join numbers are mapped in control system
- If time strings malformed: Ensure device is in MM:SS format mode (check Seconds Mode indicator)
- If programs don't trigger: Verify bidirectional joins (11, 13, 15, 17) are mapped correctly
- If communication warnings: Increase `warningTimeoutMs` gradually until stable
<!-- END Configuration Best Practices -->

---

**Generated:** January 27, 2026  
**Framework Version:** PepperDash Essentials 2.8.0  
**Plugin Version:** 1.0.0-local  
**Execution:** COPILOT_README_PROMPTS.md - Source-First Extraction with NVX Reference Standard Enforcement
