# Inviare posta elettronica sicura e garantita da Azure e servizi integrati
## Link Utili
### Microsoft HVE
[Articolo di presentazione](https://techcommunity.microsoft.com/t5/exchange-team-blog/public-preview-high-volume-email-for-microsoft-365/ba-p/4102271?wt.mc_id=M365-MVP-5005337)

[Pagina della documentazione](https://learn.microsoft.com/en-us/Exchange/mail-flow-best-practices/high-volume-mails-m365?wt.mc_id=M365-MVP-5005337)

### Azure (Email) Communication Services
[Guida generica SDK](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email?tabs=windows%2Caad&pivots=programming-language-python?wt.mc_id=M365-MVP-5005337)

[Guida generica SMTP](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email-smtp/smtp-authentication?wt.mc_id=M365-MVP-5005337)

[Autenticazione come service principal tramite le variabili d'ambiente](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#environment-variables?wt.mc_id=M365-MVP-5005337)

### SendGrid
[Repository GH dell'SDK](https://github.com/sendgrid?wt.mc_id=M365-MVP-5005337)

### Exchange & MS Graph
[Application access policy](https://learn.microsoft.com/en-us/graph/auth-limit-mailbox-access?wt.mc_id=M365-MVP-5005337)

## Informazioni utili
### Slide 17
#### Attenzione ai sottodomini nella configurazione del DKIM: 
contrariamente alle istruzioni fornite da Microsoft per la configurazione del record TXT e SPF, in questo caso nel campo "CNAME record name" viene fornito solo il nome del record che è necessario creare, tuttavia viene sottointeso che il nome deve essere inserito nel sottodominio da noi scelto come mittente delle email.
Ad esempio, se decidiamo di spedire con il dominio "globalazure.it", i record dovranno chiamarsi:
"selector1-azurecomm-prod-net._domainkey.globalazure.it" e "selector2-azurecomm-prod-net._domainkey.globalazure.it".
Mentre se dovessimo decidere di utilizzare un sottodominio, come "mailing.globalazure.it", i record da creare dovranno chiamarsi:
"selector1-azurecomm-prod-net._domainkey.mailing.globalazure.it" e "selector2-azurecomm-prod-net._domainkey.mailing.globalazure.it".
Tuttavia Azure continuerà a proporre come nome "selector1-azurecomm-prod-net._domainkey", e facendo un semplice copia-incolla, la verifica fallirà.

#### Attenzione al DMARC: 
Nel caso di implementazione del DMARC è necessario considerare che un record inserito sul dominio di livello superiore, sarà valido anche per tutti i sottodomini, ad esempio, se esiste già il record DMARC per il dominio "globalazure.it", esso verrà automaticamente utilizzato anche per "mailing.globalazure.it", ad eccezione del caso in cui si voglia configurare diversamente il protocollo per quest'ultimo e quindi venga creato un record DMARC dedicato al sottodominio.
