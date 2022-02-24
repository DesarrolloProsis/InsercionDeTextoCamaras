using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TextInsertion
{
    class IPTextInsertionManager
    {
        String tmpHostName = System.Net.Dns.GetHostName();
        private IPAddress ipContext;
        private IPAddress ipContext_2;
        private IPAddress ipDome;
        private IPAddress ipDome_2;

        public string GetLocalIPAddress()
        {
            //Obtiene la IP local de las IP's del conjunto privado, la devuelve como cadena
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                //while (!IsTheRightIPAdress(ip.ToString()))
                //{
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        //Si la IP está entre el segemento 10.3 y 10.4, continúa
                        if (IsTheRightIPAdress(ip.ToString()))
                        {
                            //ip = "192.168.0.132";
                            //String sip = "172.30.16.11";

                            //String[] IPAddressOctet = sip.Split('.');//ip.ToString().Split('.');
                            //Separa los octetos de la ip local como un array de cadenas
                            String[] IPAddressOctet = ip.ToString().Split('.');
                            int lastOctet = int.Parse(IPAddressOctet.ElementAt(3));//Último octeto de la ip local, convertido en entero
                            //Crea 4 variables. IP's basadas en la IP local (sólo cambia el último octeto)
                            String sipBC840 = IPAddressOctet[0].ToString() + "." + IPAddressOctet[1].ToString() + "." + IPAddressOctet[2].ToString() + "." + (lastOctet + 1).ToString();
                            ipContext = IPAddress.Parse(sipBC840);

                            //if (IsTresMarias(ip.ToString()))
                            //{
                            String sipBC840_2 = IPAddressOctet[0].ToString() + "." + IPAddressOctet[1].ToString() + "." + IPAddressOctet[2].ToString() + "." + (lastOctet + 2).ToString();
                            ipContext_2 = IPAddress.Parse(sipBC840_2);
                            //}

                            String sipIFD820 = IPAddressOctet[0].ToString() + "." + IPAddressOctet[1].ToString() + "." + IPAddressOctet[2].ToString() + "." + (lastOctet + 3).ToString();
                            ipDome = IPAddress.Parse(sipIFD820);

                            String sipIFD820_2 = IPAddressOctet[0].ToString() + "." + IPAddressOctet[1].ToString() + "." + IPAddressOctet[2].ToString() + "." + (lastOctet + 4).ToString();
                            ipDome_2 = IPAddress.Parse(sipIFD820_2);

                            return ip.ToString();
                        }
                    }
                //}
            }
            throw new Exception("Local IP Address Not Found!");
        }

        //True si la IP está dentro del segmento 10.3.0.0 - 10.4.254.254
        private bool IsTheRightIPAdress(String IP)
        {
            var ParsedIP = System.Net.IPAddress.Parse(IP);
            //Descompone la instancia IPAddress en un arreglo de Bytes para generar un Double equivalente a la IP
            Double lIPAddress = ParsedIP.GetAddressBytes().ElementAt(0) * Math.Pow(2,24) + ParsedIP.GetAddressBytes().ElementAt(1) * Math.Pow(2,16) + ParsedIP.GetAddressBytes().ElementAt(2) * Math.Pow(2,8) + ParsedIP.GetAddressBytes().ElementAt(3);
            if (lIPAddress >= 167968768 && lIPAddress <= 168099582) //10.3.0.0 = 167968768 - 10.4.254.254 = 168099582
            {
                TextInsertion.Logger.MessageLog("LANE IP Address:" + IP);
                return true;
            }
            else
            {
                TextInsertion.Logger.MessageLog("Not the right IP Address:" + IP);
                return false;
            }           
        }

        private bool IsTresMarias(String IP)
        {
            var ParsedIP = System.Net.IPAddress.Parse(IP);

            int ThirdOctet = ParsedIP.GetAddressBytes().ElementAt(2);
            if (ThirdOctet != 169)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public IPAddress IPCONTEXT
        {
            get { return ipContext; }
            set { ipContext = value; }
        }
        public IPAddress IPCONTEXT_2
        {
            get { return ipContext_2; }
            set { ipContext_2 = value; }
        }
        public IPAddress IPDOME
        {
            get { return ipDome; }
            set { ipDome = value; }
        }
        public IPAddress IPDOME_2
        {
            get { return ipDome_2; }
            set { ipDome_2 = value; }
        }
        
    }
}
