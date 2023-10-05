#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>

int main(int argc, char *charv[]){
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char buff[512];
	char buff2[512];

	if((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0){
		printf("Error creant socket");
	}

	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(9050);

	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		printf ("Error al bind");
	}
	//La cola de peticiones pendientes no podr? ser superior a 4 --> no sería 2 ???
	if (listen(sock_listen, 2) < 0){
		printf("Error en el Listen");
	}

	// bucle infinit per rebre peticions dels clients
	while(1){
		printf("Escuchando\n");
		//espera fins que un client realitzi una connexió
		sock_conn = accept(sock_listen, NULL, NULL);
		ret=read(sock_conn,buff, sizeof(buff));
		//posa un valor 0 al final per acabar la string
		buff[ret]='\0';

		printf("Mensaje recibido!: %s\n", buff);

		char *token = strtok(buff, "/");
		int codigo = atoi(token);

		token = strtok(NULL, "/");
		char nombre[20];
		strcpy(nombre, token);

		if(codigo == 1){
			//si el codi del missatge és 1, ens estan demanant la longitud del nom
			sprintf(buff2, "%d", (int) strlen(nombre));
		}else if(codigo == 2){
			//si el codi del missatge és 2, ens estan demanant si el nom és bonic (si comença per M o S)
			char bonic = (nombre[0] == 'M' || nombre[0] == 'S');
			sprintf(buff2, "%s", bonic ? "SI" : "NO");
		}else if(codigo == 3){
			//si el codi del missatge és 3, el tercer paràmetre correspon a l'altura i hem de retornar si és alt o no
			float altura = atof(strtok(NULL, "/"));
			sprintf(buff2, "%s", altura > 1.70 ? "SI", "NO");
		}

		//imprimeix el buffer al socket i tanca'l
		write(sock_conn,buff2, strlen(buff2));
		close(sock_conn); 
	}
}