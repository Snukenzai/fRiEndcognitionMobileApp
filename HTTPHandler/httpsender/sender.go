package httpsender

import "net"

type Sender struct {
	addr *net.TCPAddr
}

func New(addr string) (*Sender, error) {
	tcpAddr, err := net.ResolveTCPAddr("tcp", addr)
	if err != nil {
		return nil, err
	}

	return &Sender{
		addr: tcpAddr,
	}, nil
}

func (s *Sender) Send() error {
	conn, err := net.DialTCP("tcp", nil, s.addr)
	if err != nil {
		return err
	}
	defer conn.Close()

	_, err = conn.Write(nil)
	if err != nil {
		return err
	}
	return nil
}
