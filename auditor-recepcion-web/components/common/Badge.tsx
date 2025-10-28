import React from 'react';
import { Chip, ChipProps } from '@mui/material';

type BadgeVariant = 'success' | 'error' | 'warning' | 'info' | 'default';

interface BadgeProps extends Omit<ChipProps, 'color' | 'variant'> {
  variant?: BadgeVariant;
  text?: string;
}

const variantColors: Record<BadgeVariant, ChipProps['color']> = {
  success: 'success',
  error: 'error',
  warning: 'warning',
  info: 'info',
  default: 'default',
};

export const Badge: React.FC<BadgeProps> = ({ 
  variant = 'default', 
  text,
  label,
  ...props 
}) => {
  return (
    <Chip
      label={text || label}
      color={variantColors[variant]}
      size="small"
      {...props}
    />
  );
};