import React from 'react';
import { 
  Card as MuiCard, 
  CardContent, 
  CardHeader, 
  CardActions,
  CardProps as MuiCardProps,
  Divider 
} from '@mui/material';

interface CardProps extends MuiCardProps {
  title?: string;
  subtitle?: string;
  actions?: React.ReactNode;
  headerAction?: React.ReactNode;
  noPadding?: boolean;
  divided?: boolean;
}

export const Card: React.FC<CardProps> = ({
  title,
  subtitle,
  actions,
  headerAction,
  children,
  noPadding = false,
  divided = false,
  ...props
}) => {
  return (
    <MuiCard {...props}>
      {(title || subtitle) && (
        <>
          <CardHeader
            title={title}
            subheader={subtitle}
            action={headerAction}
          />
          {divided && <Divider />}
        </>
      )}
      
      <CardContent sx={{ padding: noPadding ? 0 : undefined }}>
        {children}
      </CardContent>

      {actions && (
        <>
          {divided && <Divider />}
          <CardActions>{actions}</CardActions>
        </>
      )}
    </MuiCard>
  );
};