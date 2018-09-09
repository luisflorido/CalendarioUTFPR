# CalendarioUTFPR

Este projeto foi feito para automatizar a visualização do mês com suas respectivas lições que tenha.
Fique a vontade para enviar sugestões ou partes de código, não sou um especialista em C# e sou aberto a qualquer tipo de ajuda.

## Sumário

* [Login](#login)
* [Lições](#lições)
* [Verificação](#verificações)
* [TO-DO](#to-do)
* [Instalação](#instalação)

## Login

O login é feito via **POST** para o [moodle](https://moodle.utfpr.edu.br) da UTFPR, que por si, retorna os cookies da sessão que são armazenados para efetuar login automático.

## Lições

As lições são pegas destrinchando o HTML do calendário, pegando os dias e o título das lições.

## Verificações

As verificações são feitas diariamentes as 19:00 (em breve configurável) e são apresentadas em forma de notificação as lições do dia atual ou do próximo, há também uma verificação ao abrir o programa.

## TO-DO

* ~Instalador~
* ~Inicialização automática com o windows~
* ~Notificação de lição com X dia(s) antes~
* ~Verificação diária~
* Horário configurável da verificação
* Tooltip customizável
* Aviso de provas (Integração com o portal)

## Instalação

O [instalador do programa](https://github.com/luisflorido/CalendarioUTFPR/releases/download/1.1.0/Instalador.zip) foi adicionado, para quem não tem o interesse de baixar o projeto todo basta instalar e usar, ele inicia automaticamente com o windows, em background.

***Não possuo quaisquer relações com a entidade UTFPR, o projeto não tem objetivo de afetar a universidade.***
