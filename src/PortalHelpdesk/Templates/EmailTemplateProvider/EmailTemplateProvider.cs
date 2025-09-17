using Microsoft.Build.Evaluation;
using Org.BouncyCastle.Tsp;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;

namespace PortalHelpdesk.Templates.EmailTemplateProvider
{
    public class EmailTemplateProvider : IEmailTemplateProvider
    {
        public string GetTicketAcknowledgementTemplate(Ticket ticket)
        {
            string ticketId = ticket.Id.ToString();
            string requesterName = ticket.Requester?.Name ?? "requisitante";
            string ticketTitle = ticket.Message?.Subject ?? "Sem Título";
            string ticketUrl = $"https://helpdesk.cosmuz.com/ticket/{ticket.Id}";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: O seu pedido foi criado – veja os detalhes no portal. | EN: Your request has been created – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 72px 56px; color:#555555; font-size:14px; line-height:1.8;"">
                              <h2 style=""margin-top:0; margin-bottom: 1rem; color:#333333; font-size:28px; font-weight:bold;"">
                                Acknowledgment Mail For You!
                              </h2>
                              <hr style=""border:none; border-top:2px solid #ddd; margin:12px 0;"">
              
                              <p>Caro(a) <b>{requesterName}</b>,</p>
                              <p>
                                O seu pedido foi criado com o ID: 
                                <span style=""color:#2c3244; font-weight:bold;"">#{ticketId}</span>.
                              </p>

                              <p>
                                <b>Título do pedido:</b>
                                <span style=""color:#000000; font-weight:bold;"">{ticketTitle}</span>
                              </p>

                              <p style=""margin:25px 0;"">
                                <a href=""{ticketUrl}""
                                   style=""background-color:#eb5b5b; color:#ffffff; text-decoration:none; 
                                          padding:8px 24px; border-radius:4px; font-size:16px; display:inline-block; 
                                          mso-padding-alt:0;"">
                                  View Request
                                </a>
                              </p>

                              <p style=""color:#666666; font-size:14px;"">
                                Caso necessite de mais esclarecimentos, por favor contacte-nos.
                              </p>
                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetTicketRequesterChangeTemplate(Ticket ticket)
        {
            string ticketId = ticket.Id.ToString();
            string requesterName = ticket.Requester?.Name ?? "requisitante";
            string ticketTitle = ticket.Message?.Subject ?? "Sem Título";
            string ticketUrl = $"https://helpdesk.cosmuz.com/ticket/{ticket.Id}";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: Um pedido foi atribuído a si – veja os detalhes no portal. | EN: A request has ben assigned to you – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 72px 56px; color:#555555; font-size:14px; line-height:1.8;"">
                              <h2 style=""margin-top:0; margin-bottom: 1rem; color:#333333; font-size:28px; font-weight:bold;"">
                                Acknowledgment Mail For You!
                              </h2>
                              <hr style=""border:none; border-top:2px solid #ddd; margin:12px 0;"">
              
                              <p>Caro(a) <b>{requesterName}</b>,</p>
                              <p>
                                O pedido com ID: 
                                <span style=""color:#2c3244; font-weight:bold;"">#{ticketId}</span>
                                foi atribuído a si.
                              </p>

                              <p>
                                <b>Título do pedido:</b>
                                <span style=""color:#000000; font-weight:bold;"">{ticketTitle}</span>
                              </p>

                              <p style=""margin:25px 0;"">
                                <a href=""{ticketUrl}""
                                   style=""background-color:#eb5b5b; color:#ffffff; text-decoration:none; 
                                          padding:8px 24px; border-radius:4px; font-size:16px; display:inline-block; 
                                          mso-padding-alt:0;"">
                                  View Request
                                </a>
                              </p>

                              <p style=""color:#666666; font-size:14px;"">
                                Caso necessite de mais esclarecimentos, por favor contacte-nos.
                              </p>
                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetTicketAssignedForRequesterTemplate(Ticket ticket, User assignee)
        {
            string ticketId = ticket.Id.ToString();
            string requesterName = ticket.Requester?.Name ?? "requisitante";
            string assigneeName = assignee.Name ?? "um técnico";
            string ticketUrl = $"https://helpdesk.cosmuz.com/ticket/{ticket.Id}";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: O seu pedido foi atribuído a um técnico – veja os detalhes no portal. | EN: Your request has been assigned to a technician – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 72px 56px; color:#555555; font-size:14px; line-height:1.8;"">    
                              <p>Caro(a) <b>{requesterName}</b>,</p>
                              <p>
                                O seu pedido com ID
                                <span style=""color:#555555; font-weight:bold;"">#{ticketId}</span>
                                foi atribuído a <b>{assigneeName}</b>.
                              </p>

                              <p>
                                A nossa equipa está a trabalhar na resolução do seu pedido.
                                Entraremos em contacto consigo em breve.
                                </p>

                              <p style=""margin:25px 0;"">
                                <a href=""{ticketUrl}""
                                   style=""background-color:#eb5b5b; color:#ffffff; text-decoration:none; 
                                          padding:8px 24px; border-radius:4px; font-size:16px; display:inline-block; 
                                          mso-padding-alt:0;"">
                                  View Request
                                </a>
                              </p>

                              <p style=""color:#666666; font-size:14px;"">
                                Caso necessite de mais esclarecimentos, por favor contacte-nos.
                              </p>
                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetTicketAssignedForAssigneeTemplate(Ticket ticket)
        {
            string ticketId = ticket.Id.ToString();
            string ticketTitle = ticket.Message?.Subject ?? "Sem Título";
            string requesterName = ticket.Requester?.Name ?? "requisitante";
            string priorityLevel = ticket.Priority?.Level ?? "N/A";
            string categoryName = ticket.Category?.Name ?? "N/A";
            string creatorName = ticket.Creator?.Name ?? "System";
            string ticketUrl = $"https://helpdesk.cosmuz.com/ticket/{ticket.Id}";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: Um pedido foi atribuído a si – veja os detalhes no portal. | EN: A request has ben assigned to you – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 72px 56px; color:#555555; font-size:14px; line-height:1.8;"">    
                              <p>
                                Detalhes do pedido #{ticketId}:<br>
                                Título: <b>{ticketTitle}</b>
                              </p>
                              <p>
                                Requisitante: <b>{requesterName}</b><br>
                                Prioridade: <b>{priorityLevel}</b><br>
                                Categoria: <b>{categoryName}</b><br>
                                Criado por: <b>{creatorName}</b><br>
                              </p>

                              <p style=""margin:25px 0;"">
                                <a href=""{ticketUrl}""
                                   style=""background-color:#eb5b5b; color:#ffffff; text-decoration:none; 
                                          padding:8px 24px; border-radius:4px; font-size:16px; display:inline-block; 
                                          mso-padding-alt:0;"">
                                  View Request
                                </a>
                              </p>


                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetTicketResolvedTemplate(Ticket ticket, TicketResolution resolution)
        {
            string ticketId = ticket.Id.ToString();
            string ticketTitle = ticket.Message?.Subject ?? "Sem Título";
            string resolverName = resolution.Resolver?.Name ?? "Não especificado";
            string resolutionDescription = resolution.Description ?? "Sem descrição";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: O seu pedido foi resolvido – veja os detalhes no portal. | EN: Your request has been resolved – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 24px 56px; color:#555555; font-size:14px; line-height:1.8;"">    
                              <p>
                                O seu pedido com ID #{ticketId} foi resolvido. Por favor, verifique.
                              </p>
                              <p>
                                Detalhes do pedido:<br>
                                Título: <b>{ticketTitle}</b><br>
                                Resolvido por: <b>{resolverName}</b>
                              </p>

                              <p style=""font-size: medium; margin-bottom: 0;"">
                                Resolução:<br>
                                <p style=""padding-left: 24px; border-left: #F0685A 3px solid; color: #F0685A;"">
                                    <b>{resolutionDescription}</b>
                                </p>
                              </p>

                              <p style=""margin-top: 40px; color:#666666; font-size:14px;"">
                                Se considerar que o seu pedido não ficou resolvido, responda a este e-mail para reabrir o caso.<br>
                                Este pedido será encerrado automaticamente dentro de 3 dias.
                              </p>


                            </td>
                          </tr>
                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetTicketCancelledTemplate(Ticket ticket)
        {
            string ticketId = ticket.Id.ToString();
            string ticketUrl = $"https://helpdesk.cosmuz.com/ticket/{ticket.Id}";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: O seu pedido foi cancelado – veja os detalhes no portal. | EN: Your request has been cancelled – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 24px 56px; color:#555555; font-size:14px; line-height:1.8;"">    
                              <p>
                                O seu pedido com ID <b>#{ticketId}</b> foi cancelado.
                              </p>
                              <p>
                                Por favor, não responda a este e-mail diretamente.<br>
                              </p>

                              <p style=""margin:25px 0;"">
                                <a href=""{ticketUrl}""
                                    style=""background-color:#eb5b5b; color:#ffffff; text-decoration:none; 
                                            padding:8px 24px; border-radius:4px; font-size:16px; display:inline-block; 
                                            mso-padding-alt:0;"">
                                    View Request
                                </a>
                                </p>

                                <p style=""color:#666666; font-size:14px;"">
                                Caso necessite de mais esclarecimentos, por favor contacte-nos.
                                </p>
                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetTicketClosedTemplate(Ticket ticket)
        {
            string ticketId = ticket.Id.ToString();
            string ticketTitle = ticket.Message?.Subject ?? "Sem Título";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: O seu pedido foi encerrado automaticamente – veja os detalhes no portal. | EN: Your request has been closed – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 24px 56px; color:#555555; font-size:14px; line-height:1.8;"">    
                              <p>
                                O seu pedido com ID <b>#{ticketId}</b> foi encerrado automaticamente.
                              </p>
                              <p>
                                Detalhes do pedido:<br>
                                Título: <b>{ticketTitle}</b><br>
                              </p>

                              <hr style=""border:none; border-top:2px solid #ddd; margin:12px 0;"">

                              <p>
                                Agradecemos a confiança na nossa equipa.<br>
                                Estamos sempre disponíveis para apoiar e garantir que encontra em nós um parceiro de confiança.<br>
                            </p>
                            <p>
                                Caso precise de nova assistência, não hesite em contactar-nos.
                            </p>
                            <p style=""color: #F0685A;"">
                                #BeyondTechnology
                            </p>

                            </td>
                          </tr>
                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetTicketReopenTemplate(Ticket ticket)
        {
            string ticketId = ticket.Id.ToString();
            string requesterName = ticket.Requester?.Name ?? "requisitante";
            string ticketTitle = ticket.Message?.Subject ?? "Sem Título";
            string ticketUrl = $"https://helpdesk.cosmuz.com/ticket/{ticket.Id}";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: O seu pedido foi reaberto – veja os detalhes no portal. | EN: Your request has been reopened – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 72px 56px; color:#555555; font-size:14px; line-height:1.8;"">
                              <h2 style=""margin-top:0; margin-bottom: 1rem; color:#333333; font-size:28px; font-weight:bold;"">
                                Acknowledgment Mail For You!
                              </h2>
                              <hr style=""border:none; border-top:2px solid #ddd; margin:12px 0;"">
              
                              <p>Caro(a) <b>{requesterName}</b>,</p>
                              <p>
                                O seu pedido com o ID: 
                                <span style=""color:#2c3244; font-weight:bold;"">#{ticketId}</span>
                                foi reaberto.
                              </p>

                              <p>
                                <b>Título do pedido:</b>
                                <span style=""color:#000000; font-weight:bold;"">{ticketTitle}</span>
                              </p>

                              <p style=""margin:25px 0;"">
                                <a href=""{ticketUrl}""
                                   style=""background-color:#eb5b5b; color:#ffffff; text-decoration:none; 
                                          padding:8px 24px; border-radius:4px; font-size:16px; display:inline-block; 
                                          mso-padding-alt:0;"">
                                  View Request
                                </a>
                              </p>

                              <p style=""color:#666666; font-size:14px;"">
                                Caso necessite de mais esclarecimentos, por favor contacte-nos.
                              </p>
                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetNewMessageTemplate(Ticket ticket)
        {
            string ticketId = ticket.Id.ToString();
            string ticketUrl = $"https://helpdesk.cosmuz.com/ticket/{ticket.Id}";

            return $@"
                <body style=""margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;"">
                  <style>
                        @media only screen and (max-width: 600px) {{
                        .email-container {{
                            border-radius: 0px !important;
                            width: 100% !important;
                        }}
                        .content-cell {{
                            padding: 20px !important;
                        }}
                        h2 {{
                            font-size: 22px !important;
                        }}
                        p, a {{
                            font-size: 15px !important;
                        }}
                        }}
                  </style>

                  <div style=""display:none; max-height:0; overflow:hidden; font-size:0; line-height:0; color:#ffffff;"">
                    PT: Há uma nova mensagem num pedido atribuído a si – veja os detalhes no portal. | EN: There is a new message in a request assigned to you – check the details inside.
                  </div>

                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color:#f4f4f4; padding:40px 0;"">
                    <tr>
                      <td align=""center"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""email-container"" 
                        style=""max-width:600px; width:100%; background:#ffffff; overflow:hidden; border-radius: 8px;"">

                          <!-- Header -->
                          <tr>
                            <td class=""content-cell"" bgcolor=""#0A1E40"" style=""padding:20px 56px; text-align:left;"">
                              <h1 style=""color:#ffffff; margin:0; font-size:20px; font-weight: 300;"">COSMUZ</h1>
                            </td>
                          </tr>

                          <!-- Body -->
                          <tr>
                            <td class=""content-cell"" style=""padding:24px 56px 24px 56px; color:#555555; font-size:14px; line-height:1.8;"">    
                              <p>
                                Há uma nova mensagem no pedido com ID <b>#{ticketId}</b> atribuído a si.
                              </p>
                              <p>
                                Por favor, não responda a este e-mail diretamente.<br>
                                Para responder, aceda ao portal de suporte.
                              </p>

                              <p style=""margin:25px 0;"">
                                <a href=""{ticketUrl}""
                                    style=""background-color:#eb5b5b; color:#ffffff; text-decoration:none; 
                                            padding:8px 24px; border-radius:4px; font-size:16px; display:inline-block; 
                                            mso-padding-alt:0;"">
                                    View Request
                                </a>
                                </p>

                                <p style=""color:#666666; font-size:14px;"">
                                Caso necessite de mais esclarecimentos, por favor contacte-nos.
                                </p>
                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td bgcolor=""#0A1E40"" style=""padding:24px; text-align:center; color:#ffffff; font-size:13px;"">
                              Cosmuz Support Team
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
            ";
        }

        public string GetMessageTemplate(Message message)
        {
            return $@"
                <body>
                    <div>
                        {message.Content}
                    </div>
                </body>
            ";  
        }
    }
}
