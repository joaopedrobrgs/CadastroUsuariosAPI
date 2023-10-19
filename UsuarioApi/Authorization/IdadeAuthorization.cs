using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UsuarioApi.Authorization;

public class IdadeAuthorization : AuthorizationHandler<IdadeMinima>
{
    //Implementando método da classe da qual está estendendo:
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IdadeMinima requirement)
    {
        //Olhando dentro do nosso token e pegando a Claim que corresponde à data de nascimento do usuário:
        var dataNascimentoClaim = context
            .User.FindFirst(claim => claim.Type == ClaimTypes.DateOfBirth);

        //Se não for encontrada essa claim, finalizar a tarefa sem nenhum resultado:
        if (dataNascimentoClaim is null)
        {
            return Task.CompletedTask;
        }

        //Se claim for encontrada, atribuir seu valor a uma variável, convertendo-a de string (que é tipo com o qual os objetos json são armazenados no TOKEN) para DateTime (tipo utilizado para lidar com valores de tempo):
        DateTime dataNascimento = Convert.ToDateTime(dataNascimentoClaim.Value);

        int idadeUsuario = DateTime.Today.Year - dataNascimento.Year;

        //Como tem o risco de o usuário ainda não ter feito aniversário no ano em questão, e, portanto, a sua idade ter sido calculada errada, aqui nós iremos verificar se a data de nascimento da pessoa é maior ou menor do que a data em que estamos menos a idade do Usuario (que encontramos logo acima). Se a data de nascimento for maior, vamos subtrair um ano da idade do usuário, pois significa que a pessoa não fez aniversário ainda:
        if (dataNascimento > DateTime.Today.AddYears(-idadeUsuario))
        {
            idadeUsuario--;
        }

        //Por fim, verificando se a idade do usuário é maior ou igual à idade requirida pelo politica de acesso (que até então é de 18 anos):
        if (idadeUsuario >= requirement.Idade)
        {
            context.Succeed(requirement);
        }

        //Se o requisito não for preenchido, finalizar essa tarefa sem nenhum resultado:
        return Task.CompletedTask;
    }
}
