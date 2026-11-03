import { useEffect, useRef, useState, useCallback } from 'react';
import { createConnection } from '../lib/signalr';

export function useConversation(conversationId) {
  const [events, setEvents] = useState([]);
  const [streaming, setStreaming] = useState(false);
  const [streamText, setStreamText] = useState('');
  const [done, setDone] = useState(null);
  const connRef = useRef(null);

  useEffect(() => {
    const conn = createConnection('assistant');
    conn.on('chunk', (evt) => {
      const type = evt?.type ?? evt?.Type;
      const delta = evt?.delta ?? evt?.Delta;
      const message = evt?.message ?? evt?.Message;
      const error = evt?.error ?? evt?.Error;

      if (type === 'delta' && delta) {
        setStreamText((t) => t + delta);
        return;
      }

      if (type === 'done') {
        setDone({ message, error });
        setStreaming(false);
        return;
      }

      if (type === 'blocked' || type === 'error') {
        setDone({
          message: {
            id: `err-${Date.now()}`,
            role: 'Assistant',
            content: error || 'Request blocked.',
          },
          error: error || 'Request blocked.',
        });
        setStreaming(false);
      }
    });
    conn.on('error', (err) => {
      setEvents((e) => [...e, { type: 'error', data: err }]);
      setStreaming(false);
    });
    conn.start().then(() => { connRef.current = conn; }).catch(() => {});
    return () => { conn.stop().catch(() => {}); };
  }, []);

  const send = useCallback(async (prompt) => {
    if (!connRef.current || !conversationId) return;
    setStreaming(true);
    setStreamText('');
    setDone(null);
    setEvents([]);
    try {
      await connRef.current.invoke('SendText', conversationId, prompt, false);
    } catch (err) {
      setEvents((e) => [...e, { type: 'error', data: err }]);
      setDone({
        message: {
          id: `err-${Date.now()}`,
          role: 'Assistant',
          content: 'Streaming failed. Check backend model credentials.',
        },
        error: 'Streaming failed',
      });
      setStreaming(false);
    }
  }, [conversationId]);

  return { events, streaming, streamText, done, send };
}
