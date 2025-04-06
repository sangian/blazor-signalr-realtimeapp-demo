using Microsoft.AspNetCore.Components.Forms;

namespace Frontend.Extensions;

public static class EditFormExtensions
{
    public static async Task SubmitAsync(this EditForm form)
    {
        if (form.OnSubmit.HasDelegate)
        {
            // When using OnSubmit, the developer takes control of the validation lifecycle
            await form.OnSubmit.InvokeAsync(form.EditContext);
        }
        else
        {
            // Otherwise, the system implicitly runs validation on form submission
            var isValid = form.EditContext!.Validate(); // This will likely become ValidateAsync later

            if (isValid && form.OnValidSubmit.HasDelegate)
            {
                await form.OnValidSubmit.InvokeAsync(form.EditContext);
            }

            if (!isValid && form.OnInvalidSubmit.HasDelegate)
            {
                await form.OnInvalidSubmit.InvokeAsync(form.EditContext);
            }
        }
    }
}
