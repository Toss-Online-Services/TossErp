import nodemailer from 'nodemailer';

export class EmailService {
  private transporter: nodemailer.Transporter;

  constructor(private logger: any) {
    this.transporter = nodemailer.createTransporter({
      host: process.env.SMTP_HOST || 'localhost',
      port: parseInt(process.env.SMTP_PORT || '587'),
      secure: process.env.SMTP_SECURE === 'true',
      auth: {
        user: process.env.SMTP_USER,
        pass: process.env.SMTP_PASSWORD,
      },
    });
  }

  async sendVerificationEmail(email: string, firstName: string) {
    try {
      const verificationToken = require('uuid').v4();
      const verificationUrl = `${process.env.FRONTEND_URL}/verify-email?token=${verificationToken}`;

      const mailOptions = {
        from: process.env.FROM_EMAIL || 'noreply@tosserp.com',
        to: email,
        subject: 'Welcome to TOSS ERP - Verify Your Email',
        html: `
          <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <h2 style="color: #2563eb;">Welcome to TOSS ERP, ${firstName}!</h2>
            <p>Thank you for joining TOSS ERP III, the comprehensive business management platform designed specifically for rural township enterprises.</p>
            <p>To complete your registration and secure your account, please verify your email address by clicking the button below:</p>
            <div style="text-align: center; margin: 30px 0;">
              <a href="${verificationUrl}" style="background-color: #2563eb; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;">Verify Email Address</a>
            </div>
            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
            <p style="word-break: break-all; color: #6b7280;">${verificationUrl}</p>
            <p>This verification link will expire in 24 hours for security reasons.</p>
            <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">
            <p style="color: #6b7280; font-size: 14px;">
              If you didn't create this account, please ignore this email or contact our support team if you have concerns.
            </p>
            <p style="color: #6b7280; font-size: 14px;">
              Best regards,<br>
              The TOSS ERP Team
            </p>
          </div>
        `,
      };

      await this.transporter.sendMail(mailOptions);
      this.logger.info(`Verification email sent to ${email}`);
    } catch (error) {
      this.logger.error('Failed to send verification email:', error);
    }
  }

  async sendPasswordResetEmail(email: string, firstName: string, resetToken: string) {
    try {
      const resetUrl = `${process.env.FRONTEND_URL}/reset-password?token=${resetToken}`;

      const mailOptions = {
        from: process.env.FROM_EMAIL || 'noreply@tosserp.com',
        to: email,
        subject: 'TOSS ERP - Password Reset Request',
        html: `
          <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <h2 style="color: #2563eb;">Password Reset Request</h2>
            <p>Hello ${firstName},</p>
            <p>We received a request to reset your password for your TOSS ERP account.</p>
            <p>If you made this request, click the button below to reset your password:</p>
            <div style="text-align: center; margin: 30px 0;">
              <a href="${resetUrl}" style="background-color: #dc2626; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;">Reset Password</a>
            </div>
            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
            <p style="word-break: break-all; color: #6b7280;">${resetUrl}</p>
            <p>This password reset link will expire in 1 hour for security reasons.</p>
            <p><strong>If you didn't request this password reset, please ignore this email.</strong> Your password will remain unchanged.</p>
            <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">
            <p style="color: #6b7280; font-size: 14px;">
              For security reasons, never share your password or reset links with anyone.
            </p>
            <p style="color: #6b7280; font-size: 14px;">
              Best regards,<br>
              The TOSS ERP Team
            </p>
          </div>
        `,
      };

      await this.transporter.sendMail(mailOptions);
      this.logger.info(`Password reset email sent to ${email}`);
    } catch (error) {
      this.logger.error('Failed to send password reset email:', error);
    }
  }

  async sendWelcomeEmail(email: string, firstName: string, tenantName: string) {
    try {
      const loginUrl = `${process.env.FRONTEND_URL}/login`;

      const mailOptions = {
        from: process.env.FROM_EMAIL || 'noreply@tosserp.com',
        to: email,
        subject: 'Welcome to TOSS ERP - Your Account is Ready!',
        html: `
          <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <h2 style="color: #2563eb;">Welcome to TOSS ERP, ${firstName}!</h2>
            <p>Congratulations! Your TOSS ERP account for <strong>${tenantName}</strong> has been successfully created.</p>
            <p>TOSS ERP III is designed specifically for rural township enterprises like yours, providing:</p>
            <ul style="color: #374151;">
              <li>📊 Comprehensive business management tools</li>
              <li>🤖 AI-powered business insights and automation</li>
              <li>🤝 Community networking and group buying opportunities</li>
              <li>💰 Access to credit and financing options</li>
              <li>📱 Mobile-friendly interface designed for your needs</li>
            </ul>
            <p>You can now log in to your account and start managing your business more effectively:</p>
            <div style="text-align: center; margin: 30px 0;">
              <a href="${loginUrl}" style="background-color: #059669; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;">Access Your Dashboard</a>
            </div>
            <p>If you have any questions or need assistance getting started, our support team is here to help.</p>
            <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">
            <p style="color: #6b7280; font-size: 14px;">
              Your login email: ${email}<br>
              Organization: ${tenantName}
            </p>
            <p style="color: #6b7280; font-size: 14px;">
              Best regards,<br>
              The TOSS ERP Team
            </p>
          </div>
        `,
      };

      await this.transporter.sendMail(mailOptions);
      this.logger.info(`Welcome email sent to ${email}`);
    } catch (error) {
      this.logger.error('Failed to send welcome email:', error);
    }
  }

  async sendNotificationEmail(email: string, subject: string, content: string) {
    try {
      const mailOptions = {
        from: process.env.FROM_EMAIL || 'noreply@tosserp.com',
        to: email,
        subject: `TOSS ERP - ${subject}`,
        html: `
          <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <h2 style="color: #2563eb;">${subject}</h2>
            <div style="color: #374151; line-height: 1.6;">
              ${content}
            </div>
            <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">
            <p style="color: #6b7280; font-size: 14px;">
              This is an automated notification from your TOSS ERP system.
            </p>
            <p style="color: #6b7280; font-size: 14px;">
              Best regards,<br>
              The TOSS ERP Team
            </p>
          </div>
        `,
      };

      await this.transporter.sendMail(mailOptions);
      this.logger.info(`Notification email sent to ${email}: ${subject}`);
    } catch (error) {
      this.logger.error('Failed to send notification email:', error);
    }
  }

  async testConnection(): Promise<boolean> {
    try {
      await this.transporter.verify();
      this.logger.info('Email service connection verified successfully');
      return true;
    } catch (error) {
      this.logger.error('Email service connection failed:', error);
      return false;
    }
  }
}
