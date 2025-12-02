# Checklist Workflow Update - Property Assignment

## Changes Made (November 23, 2024)

### Overview

Updated the checklist workflow to make property and lease assignment optional during creation, but required during completion. This allows users to create checklist templates/drafts without being tied to a specific property initially.

## Database Changes

### Checklist Model

- **PropertyId**: Changed from `[Required] int` to `int?` (nullable)
- LeaseId remains nullable as before
- Migration: `20251123223208_MakePropertyIdNullableInChecklist`

## Updated Workflow

### 1. Create Checklist (CreateChecklist.razor)

**Previous Behavior:**

- Required property selection
- Required lease selection (for move-in/move-out types)
- Could pass PropertyId via route parameter

**New Behavior:**

- ✅ Only requires template selection and checklist name
- ✅ Property and lease are NOT required
- ✅ Route simplified to `/propertymanagement/checklists/create` only
- ✅ Displays info message: "Property and lease will be assigned when you complete this checklist"
- ✅ Saves checklist as Draft with no property/lease assigned

### 2. Complete Checklist (CompleteChecklist.razor)

**Previous Behavior:**

- Assumed property was already assigned
- Displayed property information immediately

**New Behavior:**

- ✅ Checks if property is assigned
- ✅ If NOT assigned:
  - Shows warning card with property/lease selection dropdowns
  - Property selection is required
  - Lease selection required for Move-In/Move-Out types, optional for others
  - "Assign and Continue" button validates and saves property/lease
  - Reloads checklist to display property navigation properties
- ✅ If assigned:
  - Displays property information normally
  - Allows completing checklist items
  - Functions as before

### 3. Checklist Index (ChecklistsIndex.razor)

**Updates:**

- ✅ Displays "Not assigned" (italicized, muted) in Property column when null
- ✅ Search/filter functionality handles null property addresses gracefully
- ✅ All checklists display correctly regardless of property assignment

## User Experience

### Creating a Checklist

1. Navigate to Checklists → Create Checklist
2. Select a template (Move-In, Move-Out, Open House, Custom)
3. Enter checklist name
4. Optionally customize items (add/remove/edit)
5. Click "Create Checklist"
6. Redirected to Complete page

### Completing a Checklist

#### Scenario A: Property Not Yet Assigned

1. See warning card: "Property and Lease Required"
2. Select property from dropdown
3. If move-in/move-out: Select lease (required)
4. If open house/custom: Optionally select lease
5. Click "Assign and Continue"
6. Property card now displays property/lease info
7. Checklist items appear
8. Fill out items with status/notes
9. Save progress or mark as complete

#### Scenario B: Property Already Assigned

1. See property information card immediately
2. See checklist items
3. Fill out items with status/notes
4. Save progress or mark as complete

## Benefits

### Flexibility

- Create checklist templates in advance
- Prepare checklists before knowing which property they'll apply to
- Useful for template building and testing

### Workflow Options

1. **Template-First**: Create generic checklist → Assign to property when ready
2. **Property-Specific**: Create and assign property immediately during completion
3. **Bulk Preparation**: Create multiple checklists, assign properties later

### Use Cases

- **Property Managers**: Create standard checklists, assign when scheduling inspections
- **Template Management**: Build and test checklist templates without dummy data
- **Multi-Property**: Create one checklist type, duplicate/assign to multiple properties

## Technical Details

### Files Modified

1. **Models/Checklist.cs**

   - PropertyId: `[Required] int` → `int?`

2. **CreateChecklist.razor**

   - Removed property/lease selection UI
   - Removed PropertyId route parameter
   - Removed property/lease validation
   - Simplified initialization logic
   - Added info alert about property assignment

3. **CompleteChecklist.razor**

   - Added property/lease selection card (conditional)
   - Added property/lease loading logic
   - Added `AssignPropertyAndLease()` method
   - Added requiresLease check
   - Added `OnPropertyChanged()` for lease loading
   - Wrapped existing UI in conditional block

4. **ChecklistsIndex.razor**
   - Updated property display to show "Not assigned" when null
   - Existing search/filter already handles null gracefully

### Migration

```sql
-- 20251123223208_MakePropertyIdNullableInChecklist
ALTER TABLE Checklists
  ALTER COLUMN PropertyId INTEGER NULL;
```

## Testing Checklist

### Create Workflow

- [ ] Create checklist without selecting property
- [ ] Create checklist with all template types
- [ ] Customize items (add/remove/edit)
- [ ] Verify checklist appears in index with "Not assigned"

### Complete Workflow - No Property

- [ ] Open checklist without property
- [ ] See property/lease selection warning card
- [ ] Select property → Leases load correctly
- [ ] Move-In type: Cannot continue without lease
- [ ] Move-Out type: Cannot continue without lease
- [ ] Open House type: Can continue without lease
- [ ] Custom type: Can continue without lease
- [ ] Assign property/lease → Warning card disappears
- [ ] Checklist items appear correctly
- [ ] Complete items and save

### Complete Workflow - With Property

- [ ] Checklist displays property info immediately
- [ ] No warning card shown
- [ ] Items available immediately
- [ ] Normal completion workflow works

### Index Display

- [ ] Checklists without property show "Not assigned"
- [ ] Checklists with property show address
- [ ] Search works for both assigned and unassigned
- [ ] Filters work correctly

## Migration Notes

### Existing Data

- Existing checklists with PropertyId remain valid
- No data migration needed
- NULL PropertyId is now allowed for new checklists

### Backward Compatibility

- Viewing existing checklists works normally
- Completing existing checklists works normally
- No breaking changes to completed checklists

---

**Updated:** November 23, 2024
**Migration:** 20251123223208_MakePropertyIdNullableInChecklist
**Status:** ✅ Complete - Ready for testing
