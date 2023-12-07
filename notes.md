# Apuntes clave sobre el chat box
## Frontend (Cliente JavaScript):

### Envío de Mensajes: 
Utilizar fetch para enviar solicitudes POST al servidor con el mensaje y el nombre de usuario. Los datos se envían en formato JSON.
### Estructura del Objeto: 
Asegurar  que las claves del objeto JSON ("Messages" y "DateTime") coincidan con las que espera el servidor.
### Manejo de Respuestas: 
Procesar las respuestas del servidor y maneja los errores adecuadamente. Verificar si la respuesta tiene contenido antes de analizarla como JSON.
### Obtención de Mensajes: 
Para obtener el historial de mensajes, solicitud GET al servidor pasando el nombre de usuario en la URL.

## Backend (Servidor ASP.NET Core C#):

### Recepción de Mensajes: 
Crear un endpoint que maneje solicitudes POST para recibir mensajes. Deserializar el cuerpo de la solicitud para obtener los datos enviados.
### Almacenamiento de Mensajes: 
Actualizar el archivo JSON con los nuevos mensajes. Si el usuario no existe, crear una nueva entrada; si existe, agregar el mensaje a su lista.
### Envío de Respuestas: 
Devolver una respuesta JSON para confirmar el éxito o el error de la operación.
### Historial de Mensajes: 
Crear un endpoint que maneje solicitudes GET para devolver los mensajes de un usuario específico. Leer el archivo JSON y devolver los mensajes correspondientes al nombre de usuario solicitado.

## Puntos Importantes a Recordar:

### Consistencia: 
Mantener la consistencia en las claves y estructuras de datos entre el cliente y el servidor.
### Validación: 
Añadir validaciones para asegurar que solo se agreguen mensajes válidos y no nulos al archivo JSON.
### Pruebas: 
Asegurar que todo funcione correctamente y realizar unittest.
### Seguridad: 
Añadir medidas de seguridad adecuadas para proteger los datos y la comunicación entre el cliente y el servidor.