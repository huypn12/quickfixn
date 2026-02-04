# Use the official .NET SDK image for building and running .NET applications
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Avoid warnings by explicitly setting the locale
ENV LANG=C.UTF-8

# Install additional packages if needed
RUN apt-get update && apt-get install -y \
    git \
    curl \
    vim \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user and add them to the sudo group
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID
RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
    # [Optional] Add sudo support
    && apt-get update \
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME \
    && chmod 0440 /etc/sudoers.d/$USERNAME

# Switch to the non-root user
USER $USERNAME

# Set the working directory
WORKDIR /workspace

# Expose common .NET ports
EXPOSE 5000 5001

# Set the default command to bash
CMD ["bash"]
