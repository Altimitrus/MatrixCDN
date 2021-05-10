# Удаляем правила
/sbin/iptables -F
/sbin/iptables -X
/sbin/iptables -t nat -F
/sbin/iptables -t nat -X
/sbin/iptables -t mangle -F
/sbin/iptables -t mangle -X
/sbin/iptables -P INPUT ACCEPT
/sbin/iptables -P FORWARD ACCEPT
/sbin/iptables -P OUTPUT ACCEPT

# Разрешить локалный траффик
/sbin/iptables -A INPUT -i lo -j ACCEPT

# Белый список IP
/sbin/iptables -A INPUT -s 127.0.0.1 -j ACCEPT

# Разрешить сессии
/sbin/iptables -A INPUT -m conntrack --ctstate RELATED,ESTABLISHED -j ACCEPT

# Блокируем порты
for i in $(seq 10 40);
do
/sbin/iptables -A INPUT -p tcp --dport 10$1 -j DROP
/sbin/iptables -A INPUT -p udp --dport 10$1 -j DROP
done

iptables -L -n -v
#iptables -n -L -v --line-numbers
#iptables -D INPUT line
