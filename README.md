# CalendarioUTFPR (v1.2.1)

Este projeto foi feito para automatizar a visualização do mês com suas respectivas lições que tenha.
Fique a vontade para enviar sugestões ou partes de código, não sou um especialista em C# e sou aberto a qualquer tipo de ajuda.

## Sumário

* [Screenshots](#screenshots)
* [Login](#login)
* [Lições](#lições)
* [Provas](#provas)
* [Verificação](#verificações)
* [TO-DO](#to-do)
* [Instalação](#instalação)

## Screenshots

<p align="center">
  <img src="https://raw.githubusercontent.com/luisflorido/CalendarioUTFPR/master/Screenshots/tela_01.png"/>
</p>
<p align="center">
  <img src="https://raw.githubusercontent.com/luisflorido/CalendarioUTFPR/master/Screenshots/tela_02.png"/>
</p>
<p align="center">
  <img src="https://raw.githubusercontent.com/luisflorido/CalendarioUTFPR/master/Screenshots/tela_notify.png"/>
</p>

## Login

O login é feito via **POST** para o [moodle](https://moodle.utfpr.edu.br) da UTFPR, que por si, retorna os cookies da sessão que são armazenados para efetuar login automático.

## Lições

As lições são pegas destrinchando o HTML do calendário, pegando os dias e o título das lições.

## Provas

O sistema de provas já é um pouco mais complicado, o sistema simula o acesso ao [portal do aluno](http://portal.utfpr.edu.br/alunos/portal-do-aluno), obtem os dados do aluno, logo em seguida obtém as matérias (seu código de impressão mais especificamente), com isso têm-se a tabela de planejamento das aulas, é então procurado por 

```
Avaliação X
Prova X
```
com isto temos as datas das provas que são adicionadas ao calendário juntamente das lições.

## Verificações

As verificações são feitas diariamentes as 19:00 (em breve configurável) e são apresentadas em forma de notificação as lições do dia atual ou do próximo, há também uma verificação ao abrir o programa.

## TO-DO

* ~Instalador~
* ~Inicialização automática com o windows~
* ~Notificação de lição com X dia(s) antes~
* ~Verificação diária~
* ~Aviso de provas (Integração com o portal)~
* ~Suporte a todos os campus~
* Possibilidade de entrar somente no campus correto
* Horário configurável da verificação
* Tooltip customizável
* Indicar o nome da matéria da prova

## Instalação

O [instalador do programa](https://github.com/luisflorido/CalendarioUTFPR/releases/download/1.2.1/Instalador.zip) foi adicionado, para quem não tem o interesse de baixar o projeto todo basta instalar e usar, ele inicia automaticamente com o windows, em background.

***Não possuo quaisquer relações com a entidade UTFPR, o projeto não tem objetivo de afetar a universidade.***
