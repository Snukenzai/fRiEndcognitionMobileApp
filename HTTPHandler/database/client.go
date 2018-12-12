package database

import (
	"time"

	"github.com/boltdb/bolt"
)

type Client struct {
	Path string

	Now func() time.Time

	datacontroller DB

	db *bolt.DB
}

func NewClient() *Client {
	c := &Client{Now: time.Now}
	c.datacontroller.client = c
	return c
}

// Open opens and initializes the BoltDB database.
func (c *Client) Open() error {
	// Open database file.
	db, err := bolt.Open(c.Path, 0666, &bolt.Options{Timeout: 1 * time.Second})
	if err != nil {
		return err
	}
	c.db = db

	// Initialize top-level buckets.
	tx, err := c.db.Begin(true)
	if err != nil {
		return err
	}
	defer tx.Rollback()

	if _, err := tx.CreateBucketIfNotExists([]byte("Faces")); err != nil {
		return err
	}

	return tx.Commit()
}
func (c *Client) Close() error {
	if c.db != nil {
		return c.db.Close()
	}
	return nil
}

func (c *Client) DataController() *DB { return &c.datacontroller }
