# Checklist Feature Changelog

## 2024-11-24 - PDF Document Storage & Viewing

### Bug Fixes & Enhancements

#### PDF Generation & Storage

- **Fixed**: PDF files now properly open in browser viewer
- **Feature**: PDFs are saved to Documents table with proper linking
- **Feature**: Button changes from "Generate PDF" to "View PDF" after generation
- **Location**: `ViewChecklist.razor` - PDF generation/viewing

**Implementation Details**:

- Fixed footer styling in `ChecklistPdfGenerator` (added DefaultTextStyle)
- PDFs now saved to `Documents` table upon generation
- Document record includes:
  - Link to PropertyId (if checklist has property)
  - Link to LeaseId (if checklist has lease)
  - DocumentType: "Checklist Report"
  - FileType: "PDF"
  - Full metadata (filename, size, upload date, etc.)
- Checklist.DocumentId updated after PDF generation
- Button intelligently shows "Generate PDF" or "View PDF" based on existence

**New Behaviors**:

- **First Generation**: Generates PDF, saves to Documents table, opens in new tab, updates button
- **Subsequent Views**: Retrieves existing PDF from Documents table, opens in new tab
- Uses `viewFile` JavaScript function to open PDF in new browser tab
- No re-generation unless document is deleted
- PDF viewable even from archived checklists

**Service Methods**:

- `HandlePdfAction()` - Routes to generate or view based on DocumentId
- `GeneratePdf()` - Creates PDF, saves to DB, updates checklist
- `ViewPdf()` - Retrieves and displays existing PDF

**Benefits**:

- Single source of truth for checklist PDFs
- Reduced storage duplication
- Proper document tracking and audit trail
- PDFs linked to properties and leases for easy discovery
- Historical preservation of checklist reports

---

## 2024-11-24 - Archive System for Completed Checklists

### New Features

#### Archive/Restore Functionality

- **Feature**: Completed checklists can only be archived, not deleted
- **Location**: `ChecklistsIndex.razor` - Archive/Restore buttons
- **Functionality**:
  - Completed checklists show "Archive" button instead of "Delete"
  - Draft/In-Progress checklists can still be permanently deleted
  - Archive view accessible via "View Archive" button in header
  - Archived checklists can be restored via "Restore" button
  - Toggle between active and archived views
  - Archived checklists remain viewable but are hidden by default

**Service Methods**:

- `ArchiveChecklistAsync(int checklistId)` - Soft deletes (archives) a checklist
- `UnarchiveChecklistAsync(int checklistId)` - Restores an archived checklist
- `DeleteChecklistAsync(int checklistId)` - Now prevents deletion of completed checklists
- `GetChecklistsAsync(bool includeArchived)` - Fetches active or archived checklists

**Business Rules**:

- ✅ Completed checklists → Can only be archived
- ✅ Draft/In-Progress checklists → Can be permanently deleted
- ✅ Archived checklists → Can be restored or viewed
- ⚠️ Attempting to delete a completed checklist throws `InvalidOperationException`

**User Interface**:

- Header shows "Checklists" or "Archived Checklists" based on view
- "View Archive" button toggles to "Back to Active" when viewing archive
- "Create Checklist" button hidden when viewing archive
- Completed checklists show yellow "Archive" button
- Archived checklists show green "Restore" button
- Archive confirmation modal with informative message
- Empty state messages updated for both views

**Benefits**:

- Preserves completed checklist records for historical reference
- Prevents accidental deletion of important completed work
- Clean separation between active and archived checklists
- Easy restoration if checklist was archived by mistake

---

## 2024-11-24 - Keyword-Based Value Detection & PDF Generation

### New Features

#### 1. Keyword-Based Value Detection (Fallback)

- **Feature**: Dual approach for detecting items that require values
- **Location**: `CompleteChecklist.razor`
- **Functionality**:
  - Uses both explicit `RequiresValue` property AND keyword-based detection
  - Keyword detection acts as a fallback for items that weren't explicitly marked
  - Detects keywords: "meter reading", "reading recorded", "deposit", "amount", "forwarding address", "address obtained"
  - Ensures no value-requiring items are missed during checklist creation
  - Smart placeholders still work for both approaches

**Implementation**: `RequiresValueByKeyword(string itemText)` method provides keyword-based fallback

**Benefits**:

- More robust - catches items missed during template creation
- Backwards compatible with existing templates
- No database changes required

#### 2. PDF Generation for Checklists

- **Feature**: Generate professional PDF reports from checklists
- **Location**: `ViewChecklist.razor` - "Generate PDF" button
- **Functionality**:
  - Creates comprehensive PDF report with all checklist details
  - Includes property information, lease details, and tenant information
  - Groups items by section with formatted tables
  - Shows completion status with checkboxes (☑/☐)
  - Displays values and notes for each item
  - Includes summary statistics (total items, checked, completion %)
  - Professional header with checklist metadata
  - Footer with page numbers and generation timestamp
  - Automatic file download with descriptive filename

**Service**: `ChecklistPdfGenerator` - New service using QuestPDF

**PDF Layout**:

- Header: Checklist name, type, status, dates, property/tenant info
- Content: Grouped items in tables (checkbox, item text, value, notes)
- Summary: Statistics and completion details
- Footer: Page numbers and generation timestamp

**Technical Details**:

- Uses QuestPDF Community License
- Registered as scoped service in `Program.cs`
- Leverages existing `downloadFile` JavaScript function
- Filename format: `Checklist_{Name}_{YYYYMMDD}.pdf`

---

## 2024-11-24 - Save as Template & Hard Delete

### New Features

#### 1. Save Checklist as Template

- **Feature**: Convert any checklist into a reusable template
- **Location**: `ViewChecklist.razor` - "Save as Template" button
- **Functionality**:
  - Creates a new template from an existing checklist
  - Copies all checklist items with their `RequiresValue` settings
  - Validates template name uniqueness within the organization
  - Prevents duplicate template names with clear error messages
  - Optional template description
  - Shows count of items that will be included

**Service Method**: `ChecklistService.SaveChecklistAsTemplateAsync(int checklistId, string templateName, string? templateDescription)`

**Validation**:

- Template name is required (max 100 characters)
- Description is optional (max 500 characters)
- Template name must be unique within organization
- Throws `InvalidOperationException` for duplicate names

#### 2. Hard Delete Checklists

- **Feature**: Permanently delete checklists (not just mark as deleted)
- **Location**: `ChecklistsIndex.razor` - "Delete" button on each checklist row
- **Functionality**:
  - Shows confirmation modal with warning about permanent deletion
  - Deletes all checklist items first (cascade delete)
  - Permanently removes checklist from database
  - Displays success/error messages
  - Refreshes checklist list after deletion

**Service Method**: `ChecklistService.DeleteChecklistAsync(int checklistId)` - **CHANGED from soft delete to hard delete**

**Warning**: This is a breaking change - `DeleteChecklistAsync` now performs permanent deletion instead of setting `IsDeleted = true`

#### 3. Template Name Uniqueness Validation

- **Feature**: Ensure template names are unique within each organization
- **Location**: `ChecklistService.AddChecklistTemplateAsync()`
- **Functionality**:
  - Checks for existing template with same name in organization
  - Ignores soft-deleted templates
  - Throws `InvalidOperationException` with clear message
  - Applied to both manual template creation and "Save as Template" feature

### Technical Details

**Modified Files**:

1. `ChecklistService.cs`

   - Added `SaveChecklistAsTemplateAsync()` method
   - Changed `DeleteChecklistAsync()` from soft delete to hard delete
   - Added duplicate name validation to `AddChecklistTemplateAsync()`

2. `ViewChecklist.razor`

   - Added "Save as Template" button
   - Added modal dialog for template name/description input
   - Added form validation with DataAnnotations
   - Added success/error message handling
   - Added `SaveTemplateModel` inner class for form binding

3. `ChecklistsIndex.razor`
   - Added "Delete" button to each checklist row
   - Added delete confirmation modal with warning
   - Added success message support
   - Added `ShowDeleteConfirmation()`, `CloseDeleteConfirmation()`, and `DeleteChecklist()` methods

**Database Impact**:

- No migrations required
- `DeleteChecklistAsync()` now performs hard deletes:
  - Deletes rows from `ChecklistItems` table
  - Deletes rows from `Checklists` table
- Template name uniqueness enforced via application logic (no database constraint added)

**Error Handling**:

- Duplicate template names throw `InvalidOperationException`
- Missing checklists throw `InvalidOperationException`
- Unauthorized access throws `UnauthorizedAccessException`
- All errors displayed to user with clear messages

### User Experience

**Save as Template Flow**:

1. View any checklist
2. Click "Save as Template" button
3. Enter template name (required) and description (optional)
4. Click "Save Template"
5. Success message confirms template creation
6. Template immediately available for creating new checklists

**Delete Checklist Flow**:

1. View checklists list
2. Click "Delete" button on any checklist
3. Confirmation modal appears with warning
4. Click "Delete Permanently" to confirm
5. Success message confirms deletion
6. List refreshes automatically

### Breaking Changes

⚠️ **WARNING**: `ChecklistService.DeleteChecklistAsync()` behavior changed:

- **Before**: Soft delete (set `IsDeleted = true`)
- **After**: Hard delete (permanent removal from database)

If soft delete functionality is needed, a new method should be added (e.g., `ArchiveChecklistAsync()`).

### Future Enhancements

Potential improvements:

- Add "Archive" functionality for soft deletes
- Prevent deletion of completed checklists (business rule decision)
- Template management UI (edit existing templates)
- Bulk operations (delete multiple checklists)
- Export checklists to PDF before deletion
- Undo/restore functionality within a time window
