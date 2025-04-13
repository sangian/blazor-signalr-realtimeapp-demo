using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

public sealed class CreateNewChatDialogModel
{
    [Required(ErrorMessage = "Chat Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Chat Members is required")]
    [MinLength(1, ErrorMessage = "At least one member is required")]
    public IEnumerable<string>? Members { get; set; } = [];
}
