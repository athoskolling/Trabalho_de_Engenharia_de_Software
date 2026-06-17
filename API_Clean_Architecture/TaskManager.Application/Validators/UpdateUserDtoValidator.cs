using FluentValidation;
using TaskManager.Application.Dtos.User;
namespace TaskManager.Application.Validators;

    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
          RuleFor(x => x.Name)                                                         
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")    
            .When(x => x.Name != null);                                              
                                                                                 
        RuleFor(x => x.Email)                                                        
            .EmailAddress().WithMessage("Invalid email format.")                     
            .When(x => x.Email != null);  


        RuleFor(x => x.Password)                                                     
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")                                                                        
            .MaximumLength(20).WithMessage("Password cannot exceed 20 characters.")  
            .When(x => x.Password != null);    
        }
    }