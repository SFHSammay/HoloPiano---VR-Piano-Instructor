import asyncio
import socket

ip = '0.0.0.0'
port = 10001

CV = False
AR = False
ADDR = False

udp_port = {i: False for i in range(10010, 10020)}

def get_port():
    for p, u in udp_port.items():
        if u == False:
            udp_port[p] = True
            return p
    return -1

def creat_udp(udp_port):
    print("[logger] Creating UDP with port: {}".format(udp_port))
    udp_server = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    udp_server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    print("[logger] Binding UDP server to {}:{}".format(ip, udp_port))
    udp_server.bind((ip, udp_port))
    udp_server.setblocking(False)
    return udp_server

async def udp_connection(client, Who_am_I):
    global cv_addr, ar_addr, AR, CV, ADDR
    loop = asyncio.get_event_loop()
    while True:
        if CV == True and AR == True and ADDR == True:
            await asyncio.sleep(1)
            continue
        try:
            data, addr = client.recvfrom(1024)
            print("[status] CV = {}".format(CV))
            print("[status] VR = {}".format(AR))
            print("[logger] Recv: '{}'".format(data.decode()))
            if Who_am_I == "AR":
                ar_addr = addr
                ADDR = True
                print("[logger] VR UDP connected.")
            elif Who_am_I == "CV":
                cv_addr = addr
                print("[logger] CV UDP connected.")
            client.sendto(data, addr)
        except BlockingIOError:
            pass
        except:
            print("[logger] UDP transimit fail.")
            break
        await asyncio.sleep(0)

async def tcp_connection(client):
    global cv_client, ar_client, AR, CV, ADDR
    loop = asyncio.get_event_loop()
    Who_am_I = None
    request = (await loop.sock_recv(client, 255)).decode('utf8')
    if request[:2] == "AR":
        AR = True
        Who_am_I = "AR"
        print("[logger] VR client connected.")
    elif request[:2] == "CV":
        CV = True
        Who_am_I = "CV"
        print("[logger] CV client connected.")
    else:
        print("[logger] TCP connection close.")
        client.close()
        return

    p = get_port()
    if p == -1:
        print("[logger] TCP connection close since port error.")
        client.close()
        return
    udp_client = creat_udp(p)
    loop.create_task(udp_connection(udp_client, Who_am_I))
    if Who_am_I == "AR":
        ar_client = udp_client
    elif Who_am_I == "CV":
        cv_client = udp_client
    
    response = "UDP Port: " + str(p)
    while True:
        try:
            await loop.sock_sendall(client, response.encode('utf8'))
            request = (await loop.sock_recv(client, 255)).decode('utf8')
            response = "Hi! " + Who_am_I + " " + str(p) + "\n"
        except:
            if Who_am_I == "AR":
                AR = False
                ADDR = False
                print("[logger] VR client close.")
            elif Who_am_I == "CV":
                CV = False
                print("[logger] CV client close.")
            break

    #print("[logger] Server close.")
    udp_client.close()
    udp_port[p] = False
    client.close()

async def relay():
    global cv_client, ar_client, cv_addr, ar_addr, AR, CV, ADDR
    loop = asyncio.get_event_loop()
    while True:
        if CV != True or AR != True or ADDR != True:
            await asyncio.sleep(1)
            continue
        try:
            data, _ = cv_client.recvfrom(1024)
            #################################
            #
            data = b'hand: ' + data
            #
            #################################
            ar_client.sendto(data, ar_addr)
        except:
            pass
        await asyncio.sleep(0)

async def main():
    print("[logger] Server starting.")
    print("[logger] Creating TCP with port: {}.".format(port))
    tcp_server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    tcp_server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    print("[logger] Binding TCP server to {}:{}".format(ip, port))
    tcp_server.bind((ip, port))
    tcp_server.listen(8)
    tcp_server.setblocking(False)
    
    loop = asyncio.get_event_loop()
    loop.create_task(relay())
    while True:
        client, _ = await loop.sock_accept(tcp_server)
        loop.create_task(tcp_connection(client))

if __name__ == '__main__':
    asyncio.run(main())