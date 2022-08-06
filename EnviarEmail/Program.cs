using MailKit.Net.Smtp;
using MimeKit;

string? contactoNombre = "";
string? contactoCorreo = "";
string? contactoComentario = "";

while (string.IsNullOrEmpty(contactoNombre) || contactoNombre.Trim().Length < 1)
{
    Console.Write("Ingrese Nombre: ");
    contactoNombre = Console.ReadLine();
}
while (string.IsNullOrEmpty(contactoCorreo) || contactoCorreo.Trim().Length < 1)
{
    Console.Write("Ingrese Email: ");
    contactoCorreo = Console.ReadLine();
}
while (string.IsNullOrEmpty(contactoComentario) || contactoComentario.Trim().Length < 1)
{
    Console.WriteLine("Ingrese Comentario:");
    contactoComentario = Console.ReadLine();
}

var nombre = Environment.GetEnvironmentVariable("SEND_NOMBRE");
var email = Environment.GetEnvironmentVariable("SEND_TO");

string gmailUser = Environment.GetEnvironmentVariable("GMAIL_USER") ?? "";
string gmailPass = Environment.GetEnvironmentVariable("PASS_APP") ?? "";

var message = new MimeMessage();
message.From.Add(new MailboxAddress("Mensajeria de Contacto", gmailUser));
message.To.Add(new MailboxAddress(nombre, email));
message.Subject = "Formulario de Contacto";


//Este codigo envia solo texto plano al correo
//message.Body = new TextPart("plain")
//{
//    Text = "Los caminos de la vida"
//};


//Este codigo envia HTML al correo
BodyBuilder builder = new();
string text = string.Empty;

using( StreamReader reader = new StreamReader(Environment.CurrentDirectory + @"\plantilla.html"))
{
    text = reader.ReadToEnd();
}

text = text.Replace("{Nombre}", contactoNombre);
text = text.Replace("{Email}", contactoCorreo);
text = text.Replace("{Mensaje}", contactoComentario);

builder.HtmlBody = text;
message.Body = builder.ToMessageBody();

using (var client = new SmtpClient())
{
    client.CheckCertificateRevocation = false;
    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
    client.Authenticate(gmailUser, gmailPass); //Es necesario generar un password de aplicacion: Cuenta de Google\Seguridad\Contraseña de Aplicaciones
    client.Send(message);
    client.Disconnect(true);
}