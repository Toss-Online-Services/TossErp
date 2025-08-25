export default defineEventHandler(async (event) => {
  const method = getMethod(event)
  
  if (method !== 'POST') {
    throw createError({
      statusCode: 405,
      statusMessage: 'Method not allowed'
    })
  }

  const body = await readBody(event)
  const { conversationId, content, type = 'text', attachments = [], replyToId = null } = body

  // Validate required fields
  if (!conversationId || !content) {
    throw createError({
      statusCode: 400,
      statusMessage: 'conversationId and content are required'
    })
  }

  // Validate conversation exists (demo validation)
  const validConversations = ['conv_001', 'conv_002', 'conv_003', 'conv_004', 'conv_005']
  if (!validConversations.includes(conversationId)) {
    throw createError({
      statusCode: 404,
      statusMessage: 'Conversation not found'
    })
  }

  // Generate message ID
  const messageId = `msg_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`

  // Create message object
  const newMessage = {
    id: messageId,
    conversationId,
    senderId: 'USER_CURRENT',
    senderName: 'You',
    content,
    type,
    timestamp: new Date().toISOString(),
    isRead: false, // Will be marked as read by recipients
    attachments: attachments.map(att => ({
      id: `att_${Date.now()}_${Math.random().toString(36).substr(2, 6)}`,
      name: att.name,
      type: att.type,
      size: att.size,
      url: att.url || `/uploads/${att.name}`
    })),
    replyTo: replyToId ? {
      messageId: replyToId,
      preview: 'Original message preview...' // Would fetch actual message content
    } : null,
    status: 'sent',
    deliveryStatus: {
      sent: new Date().toISOString(),
      delivered: null,
      read: null
    },
    reactions: [],
    editHistory: []
  }

  // In a real application, this would:
  // 1. Save the message to database
  // 2. Send real-time notifications to participants
  // 3. Update conversation metadata
  // 4. Trigger any automated responses or workflows

  // Demo response with conversation update
  const updatedConversation = {
    id: conversationId,
    lastMessage: {
      id: messageId,
      senderId: 'USER_CURRENT',
      senderName: 'You',
      content,
      type,
      timestamp: new Date().toISOString(),
      isRead: false,
      attachments: newMessage.attachments
    },
    updatedAt: new Date().toISOString(),
    unreadCount: 0 // Reset for sender, but would increment for recipients
  }

  return {
    success: true,
    data: {
      message: newMessage,
      conversation: updatedConversation,
      notifications: [
        {
          type: 'message_sent',
          recipients: ['CUST-001'], // Example recipient IDs
          content: `New message in ${conversationId}`,
          timestamp: new Date().toISOString()
        }
      ]
    }
  }
})
