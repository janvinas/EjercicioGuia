#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <ctype.h>

/**
 * Retorna si la cadena de caràcters és palíndrom 1 o no 0
*/
char isPalindrome(char *text){
    int len = strlen(text);
    for(int i = 0; i < (len / 2); i++){
        if( toupper(text[i]) != toupper(text[len - 1 - i]) ) return 0;
    }

    return 1;
}

/**
 * Passa tot el text d'entrada a majúscules (modifica la string!)
*/
void toUppercase(char *text){
    for(int i = 0; i < strlen(text); i++){
        text[i] = toupper(text[i]);
    }
}


int main(int argc, char *charv[]){
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char buff[512];
	char buff2[512];

	if((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0){
		printf("Error creant socket");
		exit(-1);
	}

	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(9050);

	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		printf ("Error al bind");
		exit(-1);
	}
	//La cola de peticiones pendientes no podr? ser superior a 4 --> no sería 2 ???
	if (listen(sock_listen, 2) < 0){
		printf("Error en el Listen");
		exit(-1);
	}

	// bucle infinit per rebre peticions dels clients
	while(1){
		printf("Escuchando\n");
		//espera fins que un client realitzi una connexió
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Cliente conectado\n");
		while(1){
			ret=read(sock_conn,buff, sizeof(buff));
			//posa un valor 0 al final per acabar la string
			buff[ret]='\0';

			printf("Mensaje recibido!: %s\n", buff);

			char *token = strtok(buff, "/");
			int codigo = atoi(token);

			if(codigo == 0){
				close(sock_conn);
				printf("Cliente desconectado\n");
				break;
			}

			// per qualsevol codi diferent de 0:
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
				sprintf(buff2, "%s", altura > 1.70 ? "SI" : "NO");
			}else if(codigo == 4){
				//retorna si el nom es palíndrom 'Y'/'N'
				sprintf(buff2, "%s", isPalindrome(nombre) ? "SI" : "NO");
			}else if(codigo == 5){
				//retorna el nom en majúscules
				char copiaNombre[20];
				strcpy(copiaNombre, nombre);
				toUppercase(copiaNombre);
				sprintf(buff2, "%s", copiaNombre);
			}

			//imprimeix el buffer al socket i tanca'l
			//la resposta només s'envia si el codi del missatge no és 0
			write(sock_conn, buff2, strlen(buff2));
		}
	}
}